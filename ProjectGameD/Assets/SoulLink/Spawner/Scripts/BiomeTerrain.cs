using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if LANDSCAPE_BUILDER 
using LandscapeBuilder;
#endif

#if GRIFFIN_2021
using Pinwheel.Griffin;
#endif

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class BiomeTerrain : Biome
    {
        public FilterClause FilterClause
        {
            get { return _filterClause; }
        }

        [SerializeField]
        private FilterClause _filterClause = FilterClause.All;

        public List<TextureRuleData> TextureRules
        {
            get { return _textureRules; }
        }

        [SerializeField]
        private List<TextureRuleData> _textureRules = new List<TextureRuleData>();

        public List<VolumeRuleData> VolumeRules
        {
            get { return _volumeRules; }
        }

        [SerializeField]
        private List<VolumeRuleData> _volumeRules = new List<VolumeRuleData>();

        public List<LayerRuleData> LayerRules
        {
            get { return _layerRules; }
        }

        [SerializeField]
        private List<LayerRuleData> _layerRules = new List<LayerRuleData>();

        public bool UseElevationRange
        {
            get { return _useElevationRange; }
            set { _useElevationRange = value; }
        }
        [SerializeField]
        private bool _useElevationRange = false;

        public Vector2 ElevationRange
        { 
            get { return _elevationRange; } 
            set { _elevationRange = value; }
        }
        [SerializeField]
        private Vector2 _elevationRange = new Vector2(0f, 1000f);

        public bool texFilterFoldoutState = false;
        public bool volFilterFoldoutState = false;
        public bool layerFilterFoldoutState = false;
        public int textureSelected = 0;
        public int texRuleSelected = 0;
        public int volRuleSelected = 0;
        public int layerRuleSelected = 0;

        private bool _usePolaris = false;

#if GRIFFIN_2021
        private GStylizedTerrain[] _terrains;
#endif

#if LANDSCAPE_BUILDER 
        public List<StencilRuleData> StencilRules
        {
            get { return _stencilRules; }
        }

        [SerializeField]
        private List<StencilRuleData> _stencilRules = new List<StencilRuleData>();

        public bool stencilFilterFoldoutState = false;
        public int stencilRuleSelected = 0;
        private LBLandscape _landscape;
#endif
        public override void Initialize()
        {
#if LANDSCAPE_BUILDER 
            _landscape = FindObjectOfType<LBLandscape>();
#endif

#if GRIFFIN_2021
            _terrains = FindObjectsOfType<GStylizedTerrain>();
            _usePolaris = (_terrains != null && _terrains.Length > 0);
#endif
        } // Initialize()

        override public bool IsInBiome(Vector3 spawnPos)
        {
            bool isInBiome = false;
            if (_textureRules.Count > 0 || _volumeRules.Count > 0 || _layerRules.Count > 0)
            {
                isInBiome = CheckFilters(spawnPos);

                if (!isInBiome) return false;
            }
            else
            {
                isInBiome = true;
            }

#if LANDSCAPE_BUILDER 
            if (_stencilRules.Count > 0 && _landscape != null)
            {
                // If there are rules and the rules do not qualify then do not proceed any further
                isInBiome = CheckStencilFilters(spawnPos);
            } // if
#endif
            bool inElevation = !_useElevationRange ? true : (spawnPos.y >= _elevationRange.x && spawnPos.y <= _elevationRange.y);
            return isInBiome && inElevation;
        } // IsInBiome()

        bool CheckFilters(Vector3 spawnPos)
        {
            bool inBiome = true;
            if (_textureRules.Count > 0)
            {
                float[,,] aMap = null;

                if (_usePolaris)
                {
#if GRIFFIN_2021
                // Set the target terrain
                var targetTerrain = Utility.PolarisTerrainContaining(_terrains, spawnPos);
                if (targetTerrain == null)
                {
                    SoulLinkGlobal.DebugLog(gameObject, "IsInBiome check failed because there is no terrain at the specified position: " + spawnPos);
                    return false;
                }

                aMap = Utility.CheckPolarisTextures(targetTerrain, spawnPos);
#endif
                }
                else
                {
                    // Set the target terrain
                    var targetTerrain = Utility.TerrainContaining(spawnPos);
                    if (targetTerrain == null)
                    {
                        SoulLinkGlobal.DebugLog(gameObject, "IsInBiome check failed because there is no terrain at the specified position: " + spawnPos);
                        return false;
                    }

                    aMap = Utility.CheckTextures(targetTerrain, spawnPos);
                }


                inBiome = _filterClause == FilterClause.All ? true : false;

                // Query for just texture rules of type 'include'
                var texQuery =
                    from texData in _textureRules
                    where _textureRules.All(e => e.textureRule == FilterRule.Include)
                    select texData;

                // Verify that the aMap has all the required texture indices
                foreach (var tex in texQuery)
                {
                    // if any one item fails then cannot spawn here
                    if (aMap[0, 0, tex.textureIndex] == 0f)
                    {
                        inBiome = false;
                        break;
                    } // if
                } // foreach

                for (int i = 0; i < aMap.Length; ++i)
                {
                    var influence = aMap[0, 0, i];

                    // Check against each texture filter to see if it meets the threshhold
                    foreach (var texRule in _textureRules)
                    {
                        // If there is a texture rule for this texture index then let's verify it meets the requirements
                        if (texRule.textureIndex == i)
                        {
                            if (_filterClause == FilterClause.All)
                            {
                                if (influence >= texRule.threshold && texRule.textureRule == FilterRule.Exclude)
                                {
                                    inBiome = false;
                                    SoulLinkGlobal.DebugLog(gameObject, "IsInBiome [" + BiomeName + "] failed on exclude texture = " + texRule.textureIndex + " with threshold of " + texRule.threshold + ", influence = " + influence);
                                    return inBiome;
                                }
                            }
                            else // Any
                            {
                                if (influence <= texRule.threshold && texRule.textureRule == FilterRule.Exclude)
                                {
                                    inBiome = true;
                                    return inBiome;
                                }
                            }

                            if (_filterClause == FilterClause.All)
                            {
                                if (texRule.threshold >= influence && texRule.textureRule == FilterRule.Include)
                                {
                                    inBiome = false;
                                    SoulLinkGlobal.DebugLog(gameObject, "IsInBiome failed [" + BiomeName + "] on include texture = " + texRule.textureIndex + " with threshold of " + texRule.threshold + ", influence = " + influence);
                                    return inBiome;
                                }
                            }
                            else // Any
                            {
                                if (texRule.threshold <= influence && texRule.textureRule == FilterRule.Include)
                                {
                                    inBiome = true;
                                    return inBiome;
                                } // if
                            }
                        } // if
                    } // foreach

                    // Do not continue if any rule fails
                    if (!inBiome && _filterClause == FilterClause.All) return inBiome;
                } // for i
            }

            // Check all volume filters last; Any exclusion will cause failure, but any single inclusion will pass
            bool inVolume = false;
            foreach (var volumeRule in _volumeRules)
            {
                if (volumeRule.volume == null) continue; // ignoring null volumes

                // Exclusions are more likely so do this test first for an earlier out
                if (volumeRule.filterRule == FilterRule.Exclude && volumeRule.volume.bounds.Contains(spawnPos))
                {
                    var depth = (volumeRule.volume.bounds.max.y - spawnPos.y);

                    // Check for depth if greater than 0 to allow a spawn up to a specified depth
                    bool depthPass = true;
                    if (volumeRule.depth > 0f)
                    {
                        depthPass = (depth <= volumeRule.depth);
                    } // if

                    if (!depthPass)
                    {
                        SoulLinkGlobal.DebugLog(gameObject, "IsInBiome failed [" + BiomeName + "] on exclude volume " + volumeRule.volume.name);
                        return false;
                    } // if

                    inVolume = depthPass;
                } // if

                if (volumeRule.filterRule == FilterRule.Exclude && !volumeRule.volume.bounds.Contains(spawnPos)) inVolume = true;

                if (volumeRule.filterRule == FilterRule.Include && volumeRule.volume.bounds.Contains(spawnPos))
                {
                    var depth = (volumeRule.volume.bounds.max.y - spawnPos.y);

                    if (volumeRule.depth > 0f && depth <= volumeRule.depth)
                    {
                        inVolume = true;
                    } // if
                } // if
            } // foreach

            // Did no volume include the spawn pos?
            if (_volumeRules.Count > 0 && !inVolume)
            {
                SoulLinkGlobal.DebugLog(gameObject, "IsInBiome failed [" + BiomeName + "] on include for one or more volumes.");
                return false;
            } // if

            // Check against Layer filters
            foreach (var layerRule in _layerRules)
            {
                RaycastHit hit;
                int layerMask = 1 << LayerMask.NameToLayer(layerRule.layerName); 

                // Does the ray intersect on any objects on the specified layers?
                if (Physics.Raycast(new Vector3(spawnPos.x, spawnPos.y + SoulLinkSpawner.Instance.RaycastDistance, spawnPos.z), Vector3.down, out hit,
                    Mathf.Infinity, layerMask))
                {
                    if (layerRule.filterRule == FilterRule.Exclude)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                } // if
            } // foreach
            
            return inBiome;
        } // CheckFilterRules()

#if LANDSCAPE_BUILDER 
        bool CheckStencilFilters(Vector3 spawnPos)
        {
            var inBiome = _filterClause == FilterClause.All ? true : false;

            // Query for just texture rules of type 'include'
            var stencilQuery =
                from stencilData in _stencilRules
                where _stencilRules.All(e => e.stencilRule == FilterRule.Include)
                select stencilData;

            // Check if this has all the required stencil layers first
            foreach (var stencilRule in stencilQuery)
            {
                if (_filterClause == FilterClause.All)
                {
                    if (!stencilRule.stencil.IsPointPaintedInLayer(_landscape, stencilRule.layer, spawnPos.x, spawnPos.z, stencilRule.threshold))
                    {
                        return false;
                    } // if
                }
                else
                {
                    inBiome = stencilRule.stencil.IsPointPaintedInLayer(_landscape, stencilRule.layer, spawnPos.x, spawnPos.z, stencilRule.threshold);
                    if (inBiome) break; // at least one matched so exit include checks
                }
            } // foreach

            foreach (var stencilRule in _stencilRules)
            {
                if (_filterClause == FilterClause.All)
                {
                    if (stencilRule.stencilRule == FilterRule.Include)
                    {
                        if (!stencilRule.stencil.IsPointPaintedInLayer(_landscape, stencilRule.layer, spawnPos.x, spawnPos.z, stencilRule.threshold))
                        {
                            return false;
                        } // if
                    }
                    else // Exclude
                    {
                        if (stencilRule.stencil.IsPointPaintedInLayer(_landscape, stencilRule.layer, spawnPos.x, spawnPos.z, stencilRule.threshold))
                        {
                            return false;
                        } // if
                    }
                }
                else
                {
                    if (stencilRule.stencilRule == FilterRule.Include)
                    {
                        if (stencilRule.stencil.IsPointPaintedInLayer(_landscape, stencilRule.layer, spawnPos.x, spawnPos.z, stencilRule.threshold))
                        {
                            return true;
                        } // if
                    }
                    else // Exclude
                    {
                        if (!stencilRule.stencil.IsPointPaintedInLayer(_landscape, stencilRule.layer, spawnPos.x, spawnPos.z, stencilRule.threshold))
                        {
                            return true;
                        } // if
                    }
                }
            }

            return inBiome;
        } // CheckStencilFilters()
#endif
    } // class BiomeTerrain
} // namespace Magique.SoulLink
