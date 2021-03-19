using EctBlazorApp.Client.Pages;
using EctBlazorApp.Shared.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EctBlazorApp.ServerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CommunicationPointsComponentBaseTests
    {
        private readonly CommunicationPointsClass _componentClass;

        public CommunicationPointsComponentBaseTests()
        {
            _componentClass = new CommunicationPointsClass()
            {
                PointsAndToggles = new Dictionary<CommunicationPoint, bool>
                {
                    { new CommunicationPoint{ Medium = "Email", Points = 0 }, false },
                    { new CommunicationPoint{ Medium = "Meeting", Points = 0 }, false },
                    { new CommunicationPoint{ Medium = "Chat", Points = 0 }, false }
                }
            };
        }

        [TestInitialize]
        public void BeforeEach()
        {
            ResetPointsAndToggles();
        }

        [TestMethod]
        public void EditPercentage_NoMediumToggled_Toggled()
        {
            CommunicationPoint selectedMedium = _componentClass.PointsAndToggles.Keys.FirstOrDefault(p => p.Points == 0);

            _componentClass.EditPoints(selectedMedium);

            Assert.IsTrue(_componentClass.PointsAndToggles[selectedMedium]);
        }

        [TestMethod]
        public void EditPercentage_MediumToggledPointsInRange_ToggleSwitched()
        {
            EditPercentageToggleSwitchTest(10, 10, Assert.IsFalse, Assert.IsTrue);
        }

        [TestMethod]
        public void EditPercentage_MediumToggledPointsLessThanAZero_ToggleNotSwitched()
        {
            EditPercentageToggleSwitchTest(-1, 0, Assert.IsTrue, Assert.IsFalse);
        }

        [TestMethod]
        public void EditPercentage_MediumToggledPointsGreaterThanHundred_ToggleNotSwitched()
        {
            EditPercentageToggleSwitchTest(101, CommunicationPointsClass.MaxPointsPerMedium, Assert.IsTrue, Assert.IsFalse);
        }


        private delegate void AssertBoolean(bool value);
        private void EditPercentageToggleSwitchTest(int setPoints, int expectedPoints, AssertBoolean toggled, AssertBoolean selected)
        {
            var mediums = _componentClass.PointsAndToggles.Keys.Take(2);
            CommunicationPoint toggledMedium = mediums.ElementAt(0);
            CommunicationPoint selectedMedium = mediums.ElementAt(1);
            _componentClass.PointsAndToggles[toggledMedium] = true;
            toggledMedium.Points = setPoints;

            _componentClass.EditPoints(selectedMedium);
            int actualPoints = toggledMedium.Points;

            toggled.Invoke(_componentClass.PointsAndToggles[toggledMedium]);                                // verify that previously toggled medium is now deselected
            selected.Invoke(_componentClass.PointsAndToggles[selectedMedium]);                                // verify that the selected medium is now toggled on
            Assert.AreEqual(expectedPoints, actualPoints);
        }
        private void ResetPointsAndToggles()
        {
            foreach (var key in _componentClass.PointsAndToggles.Keys)
            {
                _componentClass.PointsAndToggles[key] = false;
                key.Points = 0;
            }
        }
    }
}
