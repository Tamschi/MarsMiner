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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MarsMiner.Shared.Geometry;

namespace MarsMiner.Shared
{
    public class MarsMinerPlugin : Plugin
    {
        protected MarsMinerPlugin( bool client, bool server )
            : base( client, server )
        {

        }

        public override void OnRegister()
        {
            if( Client )
                Register( "MarsMiner.Client.MarsMinerPlugin", Client, Server );
        }

        public override void OnWorldIntitialize( World world )
        {
            for ( int i = 1; i < 16; ++i )
            {
                BlockType sand = BlockManager.RegisterType( "MarsMiner_Sand", i - 1 );
                sand.SetComponant( new SolidityBComponant( true ) );
            }

            for ( int i = 1; i < 16; ++i )
            {
                BlockType rock = BlockManager.RegisterType( "MarsMiner_Rock", i - 1 );
                rock.SetComponant( new SolidityBComponant( true ) );
            }

            BlockType boulder = BlockManager.RegisterType( "MarsMiner_Boulder" );
            boulder.SetComponant( new SolidityBComponant( true ) );
        }
    }
}
