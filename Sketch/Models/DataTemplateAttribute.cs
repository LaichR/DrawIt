using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Models
{

    public enum TemplateTarget
    {
        DisplayTemplate,
        EditingTemplate,
        DisplayAndEditingTemplate
    };

    public class DataTemplateAttribute: Attribute
    {
        readonly string _editingTemplate;
        readonly string _displayTemplate;
        public DataTemplateAttribute(string displayDataTemplate, string editingDataTemplate)
        {
            _displayTemplate = displayDataTemplate;
            _editingTemplate = editingDataTemplate;
        }

        public DataTemplateAttribute(string templateName, TemplateTarget templateTarget )
        {
            switch(templateTarget)
            {
                case TemplateTarget.DisplayTemplate:
                    _displayTemplate = templateName;
                    break;
                case TemplateTarget.EditingTemplate:
                    _editingTemplate = templateName;
                    break;
                case TemplateTarget.DisplayAndEditingTemplate:
                    _editingTemplate = templateName;
                    _displayTemplate = templateName;
                    break;
            }
        }

        public string EditingTemplate => _editingTemplate;

        public string DisplayTemplate => _displayTemplate;
    }
}
