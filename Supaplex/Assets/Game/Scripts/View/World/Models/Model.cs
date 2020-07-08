using Game.Logic.World.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.View.World.Models
{
    public class Model
    {
        static Model[] REGISTER;

        public static void Register(ModelType modelType, Model model)
        {

        }

        static Model()
        {
            var count = (int)Enum.GetValues(typeof(ModelType)).Cast<ModelType>().Max() + 1;
            REGISTER = new Model[count];
        }
    }
}
