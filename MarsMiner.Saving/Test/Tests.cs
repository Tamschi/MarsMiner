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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarsMiner.Saving.Interface.V0;
using MarsMiner.Saving.Structures.V0;

namespace MarsMiner.Saving.Test
{
    public static class Tests
    {
        public static void TestSaving(GameSave gameSave, string saveName)
        {
            var octree = new Octree(new BitArray(new[] { false, false }), new byte[] { 1 });
            var blockTypeTable = new BlockTypeTable(
                new[] { "Block 0", "Block 1", "Block 2", "Block 2" },
                new[] { 0, 0, 0, 1 });
            var chunk = new Chunk(blockTypeTable, new[] { octree });
            var chunkTable = new ChunkTable(new[] { 0 }, new[] { 0 }, new[] { chunk });
            var mainIndex = new SavedStateIndex(DateTime.UtcNow.Ticks, saveName, chunkTable);
            var header = new Header(mainIndex);

            gameSave.Write(header, true);
        }

        public static void TestAddChunkToUnloadedChunks(GameSave gameSave)
        {
            Header oldHeader = gameSave.Read(Header.Read, new ReadOptions
                                                              {
                                                                  ChunkRead = (cx, cz, c) =>
                                                                                  {
                                                                                      Console.WriteLine(
                                                                                          "Loaded chunk at {0}",
                                                                                          c.Address);
                                                                                      c.Unload();
                                                                                  }
                                                              });

            ChunkTable oldChunkTable = oldHeader.SaveIndex.ChunkTable;

            var octree = new Octree(new BitArray(new[] { false, false }), new byte[] { 1 });
            var blockTypeTable = new BlockTypeTable(
                new[] { "Block 0", "Block 1", "Block 2", "Block 2" },
                new[] { 0, 0, 0, 1 });
            var chunk = new Chunk(blockTypeTable, new[] { octree });

            var r = new Random();

            int x = r.Next();
            int z = r.Next();

            List<Tuple<int, int, Chunk>> chunks =
                oldChunkTable.GetChunks().Where(c => c.Item1 != x || c.Item2 != z).ToList();
            chunks.Add(new Tuple<int, int, Chunk>(x, z, chunk));

            var chunkTable = new ChunkTable(chunks.ToArray());
            var mainIndex = new SavedStateIndex(DateTime.UtcNow.Ticks, "ChunkTable Length: " + chunkTable.Length,
                                                chunkTable);
            var header = new Header(mainIndex);

            gameSave.Write(header, true);
        }

        public static void TestReading(GameSave gameSave)
        {
            gameSave.Read(Header.Read, new ReadOptions());
        }

        public static void TestModify(GameSave gameSave)
        {
            Header header = gameSave.Read(Header.Read, new ReadOptions());

            var newChunkTable =
                new ChunkTable(
                    header.SaveIndex.ChunkTable.GetChunks().Concat(
                        header.SaveIndex.ChunkTable.GetChunks().Take(1).Select(
                            x => new Tuple<int, int, Chunk>(x.Item1 + 1, x.Item2 - 1, x.Item3))).ToArray());

            var newHeader = new Header(new SavedStateIndex(DateTime.UtcNow.Ticks, "Modified Save", newChunkTable));

            gameSave.Write(newHeader, true);
        }

        public static void TestResave(GameSave gameSave)
        {
            Header header = gameSave.Read(Header.Read, new ReadOptions());
            gameSave.Write(header, true);
        }

        public static void TestMarkModify(GameSave gameSave)
        {
            Header header = gameSave.Read(Header.Read, new ReadOptions());

            var newChunkTable =
                new ChunkTable(
                    header.SaveIndex.ChunkTable.GetChunks().Concat(
                        header.SaveIndex.ChunkTable.GetChunks().Take(1).Select(
                            x => new Tuple<int, int, Chunk>(x.Item1 + 1, x.Item2 - 1, x.Item3))).ToArray());

            var newHeader = new Header(new SavedStateIndex(DateTime.UtcNow.Ticks, "Modified Save", newChunkTable));

            gameSave.Write(newHeader, true);
        }
    }
}