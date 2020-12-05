using System.Collections.Generic;
using System.Linq;
using UnityEngine;


    public class BuildingModelRoot : ForbidLocalMoveRoot
    {
        [SerializeField]
        private List<BuildingModel> Models = new List<BuildingModel>();

        private Animator ModelAnimator;

        void Awake()
        {
            Models = GetComponentsInChildren<BuildingModel>().ToList();
            ModelAnimator = GetComponent<Animator>();
        }

        public void SetShown(bool shown)
        {
            foreach (BuildingModel model in Models)
            {
                model.SetShown(shown);
            }
        }

        public void OnDamage(float portion)
        {
            foreach (BuildingModel model in Models)
            {
                model.OnDamage(portion);
            }
        }

        public void OnPowerChange(float portion)
        {
            foreach (BuildingModel model in Models)
            {
                model.OnPowerChange(portion);
            }
        }

        public void SetBuildingBasicEmissionColor(Color basicEmissionColor)
        {
            foreach (BuildingModel model in Models)
            {
                model.SetBuildingBasicEmissionColor(basicEmissionColor);
            }
        }

        public void ResetBuildingBasicEmissionColor()
        {
            foreach (BuildingModel model in Models)
            {
                model.ResetBuildingBasicEmissionColor();
            }
        }

        public void SetDefaultHighLightEmissionColor(Color highLightEmissionColor)
        {
            foreach (BuildingModel model in Models)
            {
                model.SetDefaultHighLightEmissionColor(highLightEmissionColor);
            }
        }

        public void ResetColor()
        {
            foreach (BuildingModel model in Models)
            {
                model.ResetColor();
            }
        }

        public void SetAnimTrigger(string trigger)
        {
            if (ModelAnimator) ModelAnimator.SetTrigger(trigger);
        }
    }
