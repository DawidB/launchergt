using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LauncherGTLib
{
    public sealed class LauncherGT
    {
        private static LauncherGTBase instance = null;
        private static readonly object padlock = new object();

        LauncherGT()
        {
        }

        public static LauncherGTBase Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new LauncherGTBase();
                    return instance;
                }
            }
        }
    }
}
