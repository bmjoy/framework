﻿using ActionStreetMap.Core.Geometry.Triangle.Geometry;

namespace ActionStreetMap.Core.Geometry.Triangle.Meshing
{
    /// <summary> Interface for polygon triangulation. </summary>
    public interface IConstraintMesher
    {
        /// <summary> Triangulates a polygon. </summary>
        /// <param name="polygon"> The polygon. </param>
        /// <returns> Mesh </returns>
        Mesh Triangulate(IPolygon polygon);

        /// <summary> Triangulates a polygon, applying constraint options. </summary>
        /// <param name="polygon"> The polygon. </param>
        /// <param name="options"> Constraint options. </param>
        /// <returns> Mesh </returns>
        Mesh Triangulate(IPolygon polygon, ConstraintOptions options);
    }
}