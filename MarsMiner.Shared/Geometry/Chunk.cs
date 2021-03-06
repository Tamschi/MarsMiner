﻿/**
 * Copyright (c) 2012 James King [metapyziks@gmail.com]
 *
 * This file is part of MarsMiner.
 * 
 * MarsMiner is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * MarsMiner is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with MarsMiner. If not, see <http://www.gnu.org/licenses/>.
 */

using System;

using MarsMiner.Shared.Octree;

namespace MarsMiner.Shared.Geometry
{
    public class Chunk : IOctreeContainer<UInt16>
    {
        public const int Size = 64;
        public const int Height = 1024;

        public Octree<UInt16>[] Octrees { get; private set; }

        public readonly World World;
        public readonly int X;
        public readonly int Z;

        public int DistanceToOrigin
        {
            get
            {
                return Math.Max(
                    Math.Abs( X ) - ( X < 0 ? Size : 0 ),
                    Math.Abs( Z ) - ( Z < 0 ? Size : 0 ) );
            }
        }

        public bool Loaded { get; private set; }

        public Chunk( World world, int x, int z )
        {
            World = world;

            X = x;
            Z = z;

            Loaded = false;
        }

        public void Generate( int resolution )
        {
            Loaded = false;

            int octrees = Height / Size;
            Octrees = new Octree<UInt16>[ octrees ];

            int dist = DistanceToOrigin;

            for ( int i = 0; i < octrees; ++i )
            {
                Octree<UInt16> newTree = World.Generator.Generate( X, i * Size, Z, Size, resolution );
                newTree.Container = this;
                Octrees[ i ] = newTree;
            }

            Loaded = true;
        }

        public OctreeNode<UInt16> FindNode( int x, int y, int z, int size )
        {
            if ( x < X || x >= X + Size || z < Z || z >= Z + Size )
                return World.FindNode( x, y, z, size );

            if ( y < 0 || y >= Height || !Loaded )
                return null;

            return Octrees[ y / Size ].FindNode( x, y, z, size );
        }
    }
}
