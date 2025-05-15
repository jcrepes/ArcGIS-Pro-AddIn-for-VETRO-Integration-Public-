using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VETRO_Integration
{
    internal class ShowVETRO_IntegrationWindow : Button
    {

        private VETRO_IntegrationWindow _vetro_integrationwindow = null;

        protected override void OnClick()
        {
            //already open?
            if (_vetro_integrationwindow != null)
                return;
            _vetro_integrationwindow = new VETRO_IntegrationWindow();
            _vetro_integrationwindow.Owner = FrameworkApplication.Current.MainWindow;
            _vetro_integrationwindow.Closed += (o, e) => { _vetro_integrationwindow = null; };
            _vetro_integrationwindow.Show();
            //uncomment for modal
            //_vetro_integrationwindow.ShowDialog();
        }

    }
}
