using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bluebottle.Base.Controls.EnvironmentVariablesBox
{
    /// <summary>
    /// Interaction logic for EnvVariablesBox.xaml
    /// </summary>
    public partial class EnvVariablesBox : Window
    {
        public EnvVariablesBox()
        {
            InitializeComponent();
            DataContext = this;
        } 

        public Dictionary<string, string> EnvVariables
        {
            get 
            {
                try
                {
                    Dictionary<string, string> variables = new Dictionary<string, string>();
                    //add paths env. variables mentioned in bootstrapper
                    var envVarsBootstrapper = new List<string>(){"HIMS_DEBUGGER","FSL_CONFIG", "PY_HI"};
                    foreach(var envV in envVarsBootstrapper)
                    {
                        if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable(envV)))
                            variables.Add(envV, Environment.GetEnvironmentVariable(envV));
                    }
                    //project referenced paths
                    Parallel.ForEach<DictionaryEntry>(Environment.GetEnvironmentVariables().OfType<DictionaryEntry>(), entry =>
                        {
                            if (entry.Key.ToString().EndsWith("_PATH"))
                                variables.Add(entry.Key.ToString(), entry.Value.ToString());
                        });
                    return variables;
                }
                catch (SecurityException ex)
                {
                    Console.WriteLine("Error retrieving environment variables: {0}", ex.Message);
                    return null;
                }            
            }
        }
    }
}
