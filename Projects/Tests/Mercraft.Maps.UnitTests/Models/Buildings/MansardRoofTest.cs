﻿using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Roofs;
using NUnit.Framework;
using UnityEngine;
using Rect = Mercraft.Models.Geometry.Rect;

namespace Mercraft.Maps.UnitTests.Models.Buildings
{
    [TestFixture]
    public class MansardRoofTest
    {
        [Test]
        public void CanBuildMansardWithValidData()
        {
            // ARRANGE
            var builder = new MansardRoofBuilder();

            // ACT
            var meshData = builder.Build(new Building()
            {
                Footprint = new List<MapPoint>()
                {
                    new MapPoint(0, 0),
                    new MapPoint(0, 5),
                    new MapPoint(5, 5),
                    new MapPoint(5, 0),
                },
                Elevation = 0,
                Height = 1
            }, new BuildingStyle()
            {
                Roof = new BuildingStyle.RoofStyle()
                {
                    Builders = new IRoofBuilder[] { new FlatRoofBuilder(), },
                    FrontUvMap = new Rect(new Vector2(), new Vector2()),
                    Material = "",
                },
                Facade = new BuildingStyle.FacadeStyle()
            });

            // ASSERT

            Assert.AreEqual(20, meshData.Vertices.Length);
            Assert.AreEqual(30, meshData.Triangles.Length);
            Assert.AreEqual(20, meshData.UV.Length);
        }
    }
}
