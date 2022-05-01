using Newtonsoft.Json.Linq;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.Avalonia.ViewModels
{
    

    [ClassPropertyChangedAvalonia(typeof(TestProperties))]
    public partial class TestViewModel : ViewModelBase
    {
        private class TestProperties
        {
            public string ABC { get; set; }
            public int DEF { get; set; }
        }

        [PropertyChangedAvalonia(PropertyName ="NoTest")]
        private bool testPropChanged;
        public TestViewModel()
        {
        }
    }

    public class TestForView
    {
        public TestForView()
        {
            var tvm = new TestViewModel();
            //tvm.ABC = "";
        }
    }
}
