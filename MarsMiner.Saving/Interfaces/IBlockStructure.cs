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
using System.IO;
using MarsMiner.Saving.Util;

namespace MarsMiner.Saving.Interfaces
{
    public interface IBlockStructure
    {
        int Length { get; }

        Tuple<int, uint> Address { get; set; }

        IBlockStructure[] UnboundBlocks { get; }

        Dictionary<int, IntRangeList> RecursiveUsedSpace { get; }

        void Write(Stream stream, Func<IBlockStructure, IBlockStructure, uint> getBlockPointerFunc,
                   Func<string, uint> getStringPointerFunc);

        void Unload();
    }
}