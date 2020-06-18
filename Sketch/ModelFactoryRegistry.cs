using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sketch.Interface;

namespace Sketch
{
    public class ModelFactoryRegistry
    {

        static readonly ModelFactoryRegistry _instance = new ModelFactoryRegistry();

        public static ModelFactoryRegistry Instance
        {
            get => _instance;
        }

        Stack<ISketchItemFactory> _factoryStack = new Stack<ISketchItemFactory>();

        private ModelFactoryRegistry() { }

        public void PushSketchItemFactory( ISketchItemFactory factory )
        {
            _factoryStack.Push(factory);
        }

        public ISketchItemFactory PopSketchItemFactory()
        {
            return _factoryStack.Pop();
        }

        public ISketchItemFactory GetSketchItemFactory()
        {
            return _factoryStack.Peek();
        }

    }
}
