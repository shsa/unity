using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game
{
    public class Model
    {
        public ModelTypeEnum type;
        public ModelEnum model;

        public Model(ModelTypeEnum type, ModelEnum model)
        {
            this.type = type;
            this.model = model;
        }

        public override int GetHashCode()
        {
            return (model.GetHashCode() << 2) ^ type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var m = obj as Model;
            return m.model == model && m.type == type;
        }

        public override string ToString()
        {
            return $"{type}.{model}";
        }
    }

    public class SimpleModel : Model
    {
        public SimpleModel(ModelEnum model) : base(ModelTypeEnum.Simple, model)
        {

        }
    }

    public class TileSetModel : Model
    {
        public TileSetModel(ModelEnum model) : base(ModelTypeEnum.TileSet, model)
        {

        }
    }
}
