﻿/**
 * Copyright (c) 2012 Tamme Schichler [tammeschichler@googlemail.com]
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarsMiner.Saving.Interfaces;
using System.IO;

namespace MarsMiner.Saving.Structures.V0
{
    internal class ChunkTable : IBlockStructure
    {
        private int[] xLocations;
        private int[] zLocations;
        private Chunk[] chunks;

        private Tuple<int, uint> address;
        public Tuple<int, uint> Address
        {
            get
            {
                return address;
            }
            set
            {
                if (address != null)
                {
                    throw new InvalidOperationException("Address can't be reassigned!");
                }
                address = value;
            }
        }

        public IEnumerable<Tuple<int, int, Chunk>> GetChunks()
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                yield return new Tuple<int, int, Chunk>(xLocations[i], zLocations[i], chunks[i]);
            }
        }

        public ChunkTable(IEnumerable<Tuple<int, int, Chunk>> chunks)
            : this(
                chunks.Select(x => x.Item1).ToArray(),
                chunks.Select(x => x.Item2).ToArray(),
                chunks.Select(x => x.Item3).ToArray())
        { }

        public ChunkTable(int[] xLocations, int[] zLocations, Chunk[] chunks)
        {
            if (xLocations.Length != zLocations.Length || zLocations.Length != chunks.Length)
            {
                throw new ArgumentException("Argument arrays must have the same length!");
            }
            this.xLocations = xLocations;
            this.zLocations = zLocations;
            this.chunks = chunks;
        }

        private ChunkTable(int[] xLocations, int[] zLocations, Chunk[] chunks, Tuple<int, uint> address)
            : this(xLocations, zLocations, chunks)
        {
            Address = address;
        }
        #region IBlockStructure
        public int Length
        {
            get
            {
                return 4 //chunk count

                    + chunks.Length *
                    (4 // xLocation
                    + 4 // yLocation
                    + 4); // chunk
            }
        }

        public void Write(Stream stream, Func<IBlockStructure, IBlockStructure, uint> getBlockPointerFunc, Func<string, uint> getStringPointerFunc)
        {
#if AssertBlockLength
            var start = stream.Position;
#endif
            var w = new BinaryWriter(stream);

            w.Write((uint)chunks.LongLength);
            for (long i = 0; i < chunks.LongLength; i++)
            {
                w.Write(xLocations[i]);
                w.Write(zLocations[i]);
                var chunkPointer = getBlockPointerFunc(this, chunks[i]);
                w.Write(chunkPointer);
            }
#if AssertBlockLength
            if (stream.Position - start != Length)
            {
                throw new Exception("Length mismatch in ChunkTable!");
            }
#endif
        }
        #endregion

        public static ChunkTable Read(Tuple<int, uint> source, Func<int, uint, Tuple<int, uint>> resolvePointerFunc, Func<uint, string> resolveStringFunc, Func<int, Stream> getStreamFunc)
        {
            var stream = getStreamFunc(source.Item1);
            stream.Seek(source.Item2, SeekOrigin.Begin);
            var r = new BinaryReader(stream);

            var chunkCount = r.ReadUInt32();

            var xLocations = new int[chunkCount];
            var yLocations = new int[chunkCount];
            var chunkPointers = new uint[chunkCount];

            for (int i = 0; i < chunkCount; i++)
            {
                xLocations[i] = r.ReadInt32();
                yLocations[i] = r.ReadInt32();
                chunkPointers[i] = r.ReadUInt32();
            }

            var chunks = new Chunk[chunkCount];

            for (int i = 0; i < chunkCount; i++)
            {
                chunks[i] = Chunk.Read(resolvePointerFunc(source.Item1, chunkPointers[i]), resolvePointerFunc, resolveStringFunc, getStreamFunc);
            }

            return new ChunkTable(xLocations, yLocations, chunks, source);
        }
    }
}
