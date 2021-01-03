using TileMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIWorldChanger : MonoBehaviour {
        public WorldMap worldMap;

        public TMP_Dropdown worldDropDown;
        public Slider dimensionSlider;
        public Slider heightMultiplierSlider;
        public Slider scaleSlider;

        public Slider octavesSlider;
        public Slider persistenceSlider;
        public Slider lacunaritySlider;

        public Slider seedSlider;
        public Slider xOffsetSlider;
        public Slider yOffsetSlider;

        public Toggle useFalloffToggle;
        public Slider falloffMultiplierSlider;
        public Slider falloffModifier1Slider;
        public Slider falloffModifier2Slider;

        public Toggle jaggedToggle;
        public Slider jaggedPercentSlider;
        
        public void Start() {
            ReloadUI();
        }

        private void ReloadUI() {
            //Change Handlers removed to prevent the slider value updates cause the World to Generate
            RemoveChangeHandlers();
            InitUIValues();
            InitChangeHandlers();
        }
        /// <summary>
        /// Set the UI values based on the selected world
        /// </summary>
        private void InitUIValues() {
            dimensionSlider.value = worldMap.m_HeightSettings.dimensionLength;
            SetSliderValueAsText(dimensionSlider);
            
            heightMultiplierSlider.value = worldMap.m_HeightSettings.heightMultiplier;
            SetSliderValueAsText(heightMultiplierSlider);
            
            scaleSlider.value = worldMap.m_HeightSettings.scale;
            SetSliderValueAsText(scaleSlider);

            octavesSlider.value = worldMap.m_HeightSettings.octaves;
            SetSliderValueAsText(octavesSlider);
            
            persistenceSlider.value = worldMap.m_HeightSettings.persistence;
            SetSliderValueAsText(persistenceSlider);
            
            lacunaritySlider.value = worldMap.m_HeightSettings.lacunarity;
            SetSliderValueAsText(lacunaritySlider);

            seedSlider.value = worldMap.m_HeightSettings.seed;
            SetSliderValueAsText(seedSlider);

            xOffsetSlider.value = worldMap.m_HeightSettings.offset.x;
            SetSliderValueAsText(xOffsetSlider);
            
            yOffsetSlider.value = worldMap.m_HeightSettings.offset.y;
            SetSliderValueAsText(yOffsetSlider);

            useFalloffToggle.isOn = worldMap.m_HeightSettings.useFalloff;
            falloffMultiplierSlider.value = worldMap.m_HeightSettings.falloffMultiplier;
            SetSliderValueAsText(falloffMultiplierSlider);
            falloffModifier1Slider.value = worldMap.m_HeightSettings.falloffModifier1;
            SetSliderValueAsText(falloffModifier1Slider);
            falloffModifier2Slider.value = worldMap.m_HeightSettings.falloffModifier2;
            SetSliderValueAsText(falloffModifier2Slider);

            jaggedToggle.isOn = worldMap.m_HeightSettings.jagged;
            jaggedPercentSlider.value = worldMap.m_HeightSettings.jaggedPercent;
            SetSliderValueAsText(jaggedPercentSlider);
        }

        /// <summary>
        /// Add listeners to each UI component the user can interact with, triggers delegate method when a value changes
        /// </summary>
        private void InitChangeHandlers() {
            worldDropDown.onValueChanged.AddListener((delegate { WorldChanged(); }));
            dimensionSlider.onValueChanged.AddListener(delegate { DimensionValueChanged(); });
            heightMultiplierSlider.onValueChanged.AddListener(delegate { HeightMultiplierValueChanged(); });
            scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChanged(); });

            octavesSlider.onValueChanged.AddListener(delegate { OctavesValueChanged(); });
            persistenceSlider.onValueChanged.AddListener(delegate { PersistenseValueChanged(); });
            lacunaritySlider.onValueChanged.AddListener(delegate { LacunarityValueChanged(); });

            seedSlider.onValueChanged.AddListener(delegate { SeedValueChanged(); });

            xOffsetSlider.onValueChanged.AddListener(delegate { XOffsetValueChanged(); });
            yOffsetSlider.onValueChanged.AddListener(delegate { YOffsetValueChanged(); });

            useFalloffToggle.onValueChanged.AddListener(delegate { UseFalloutValueChanged(); });
            falloffMultiplierSlider.onValueChanged.AddListener(delegate { FalloffMultiplierValueChanged(); });
            falloffModifier1Slider.onValueChanged.AddListener(delegate { FalloffModifier1Changed(); });
            falloffModifier2Slider.onValueChanged.AddListener(delegate { FalloffModifier2Changed(); });

            jaggedToggle.onValueChanged.AddListener(delegate { JaggedValueChanged(); });
            jaggedPercentSlider.onValueChanged.AddListener(delegate { JaggedPercentValueChanged(); });
        }

        private void RemoveChangeHandlers() {
            worldDropDown.onValueChanged.RemoveAllListeners();
            dimensionSlider.onValueChanged.RemoveAllListeners();
            heightMultiplierSlider.onValueChanged.RemoveAllListeners();
            scaleSlider.onValueChanged.RemoveAllListeners();

            octavesSlider.onValueChanged.RemoveAllListeners();
            persistenceSlider.onValueChanged.RemoveAllListeners();
            lacunaritySlider.onValueChanged.RemoveAllListeners();

            seedSlider.onValueChanged.RemoveAllListeners();

            xOffsetSlider.onValueChanged.RemoveAllListeners();
            yOffsetSlider.onValueChanged.RemoveAllListeners();

            useFalloffToggle.onValueChanged.RemoveAllListeners();
            falloffMultiplierSlider.onValueChanged.RemoveAllListeners();
            falloffModifier1Slider.onValueChanged.RemoveAllListeners();
            falloffModifier2Slider.onValueChanged.RemoveAllListeners();

            jaggedToggle.onValueChanged.RemoveAllListeners();
            jaggedPercentSlider.onValueChanged.RemoveAllListeners();
        }
        private TMP_Text GetUITextComponent(Component parentComponent) {
            TMP_Text[] textComponents = parentComponent.GetComponentsInChildren<TMP_Text>();
            //Based on Component Order in the Scene, the Value Text is guaranteed to be the second element
            //If this ever fails, just do a Name compare on "ValueAsText"
            return textComponents[1];  
        }
        
        private void SetSliderValueAsText(Slider slider) {
            TMP_Text textComponent = GetUITextComponent(slider);
            if (slider.value % 1 == 0) { //is a float but a whole number
                textComponent.SetText(slider.value.ToString());
            }
            else {
                // Only display up to 2 decimal places for floats
                textComponent.SetText(slider.value.ToString("F2"));
            }
        }
        
        /** These methods are invoked when the values of the sliders ared changed. **/
        private void WorldChanged() {
            string worldAsText = worldDropDown.options[worldDropDown.value].text;
            World world;
            World.TryParse(worldAsText, out world);
            worldMap.world = world;

            worldMap.LoadWorldResources();
            ReloadUI();
            worldMap.GenerateMap();
        }
        
        private void DimensionValueChanged() {
            worldMap.m_HeightSettings.dimensionLength = (int) dimensionSlider.value;
            SetSliderValueAsText(dimensionSlider);
            
            worldMap.GenerateMap();
        }
        
        private void HeightMultiplierValueChanged() {
            worldMap.m_HeightSettings.heightMultiplier = (int) heightMultiplierSlider.value;
            SetSliderValueAsText(heightMultiplierSlider);
            worldMap.GenerateMap();
        }

        private void ScaleValueChanged() {
            worldMap.m_HeightSettings.scale = scaleSlider.value;
            SetSliderValueAsText(scaleSlider);
            worldMap.GenerateMap();
        }

        private void OctavesValueChanged() {
            worldMap.m_HeightSettings.octaves = (int) octavesSlider.value;
            SetSliderValueAsText(octavesSlider);
            worldMap.GenerateMap();
        }

        private void LacunarityValueChanged() {
            worldMap.m_HeightSettings.lacunarity = lacunaritySlider.value;
            SetSliderValueAsText(lacunaritySlider);
            worldMap.GenerateMap();
        }

        private void PersistenseValueChanged() {
            worldMap.m_HeightSettings.persistence = persistenceSlider.value;
            SetSliderValueAsText(persistenceSlider);
            worldMap.GenerateMap();
        }

        private void SeedValueChanged() {
            worldMap.m_HeightSettings.seed = (int) seedSlider.value;
            SetSliderValueAsText(seedSlider);
            worldMap.GenerateMap();
        }

        private void XOffsetValueChanged() {
            Vector2 currentOffset = worldMap.m_HeightSettings.offset;
            currentOffset.x = xOffsetSlider.value;
            worldMap.m_HeightSettings.offset = currentOffset;
            
            SetSliderValueAsText(xOffsetSlider);
            worldMap.GenerateMap();
        }

        private void YOffsetValueChanged() {
            Vector2 currentOffset = worldMap.m_HeightSettings.offset;
            currentOffset.y = yOffsetSlider.value;
            worldMap.m_HeightSettings.offset = currentOffset;
            
            SetSliderValueAsText(yOffsetSlider);
            worldMap.GenerateMap();
        }

        private void UseFalloutValueChanged() {
            worldMap.m_HeightSettings.useFalloff = useFalloffToggle.isOn;
            worldMap.GenerateMap();
        }

        private void FalloffMultiplierValueChanged() {
            worldMap.m_HeightSettings.falloffMultiplier = falloffMultiplierSlider.value;
            SetSliderValueAsText(falloffMultiplierSlider);
            worldMap.GenerateMap();
        }

        private void FalloffModifier1Changed() {
            worldMap.m_HeightSettings.falloffModifier1 = falloffModifier1Slider.value;
            SetSliderValueAsText(falloffModifier1Slider);
            worldMap.GenerateMap();
        }

        private void FalloffModifier2Changed() {
            worldMap.m_HeightSettings.falloffModifier2 = falloffModifier2Slider.value;
            SetSliderValueAsText(falloffModifier2Slider);
            worldMap.GenerateMap();
        }

        private void JaggedValueChanged() {
            worldMap.m_HeightSettings.jagged = jaggedToggle.isOn;
            worldMap.GenerateMap();
        }

        private void JaggedPercentValueChanged() {
            worldMap.m_HeightSettings.jaggedPercent = jaggedPercentSlider.value;
            SetSliderValueAsText(jaggedPercentSlider);
            worldMap.GenerateMap();
        }
    }
}