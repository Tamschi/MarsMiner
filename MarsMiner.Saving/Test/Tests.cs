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
using MarsMiner.Saving.Structures.V0;
using System.Collections;

namespace MarsMiner.Saving.Test
{
    public static class Tests
    {
        public static void TestSaving(GameSave gameSave, string saveName)
        {
            {
                //Write test
                var octree = new Octree(new BitArray(new bool[] { false, false }), new byte[] { 1 });
                var blockTypeTable = new BlockTypeTable(new string[] { "Block 0", "Block 1", "Block 2" });
                var chunk = new Chunk(blockTypeTable, new Octree[] { octree });
                var chunkTable = new ChunkTable(new int[1] { 0 }, new int[1] { 0 }, new Chunk[1] { chunk });
                var mainIndex = new SavedStateIndex(DateTime.UtcNow.Ticks, saveName, chunkTable);
                var header = new Header(mainIndex);

                gameSave.WriteTransaction(
                    new Cache.WriteTransaction(header,
                    new Interfaces.IBlockStructure[] { 
                    mainIndex,
                    chunkTable,
                    chunk,
                    blockTypeTable,
                    octree}));
            }

            {
                //Read test
                gameSave.Read(Header.Read);
            }
        }
    }
}