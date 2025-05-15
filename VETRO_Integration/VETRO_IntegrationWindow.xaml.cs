using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Core.Internal.CIM;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VETRO_Integration
{
    /// <summary>
    /// Interaction logic for VETRO_IntegrationWindow.xaml
    /// </summary>
    public partial class VETRO_IntegrationWindow : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        private System.Windows.Controls.ComboBox _mapLayersComboBox;
        private System.Windows.Controls.TextBox _apiTokenTextBox;
        private System.Windows.Controls.Button _apiSubmitButton;
        private System.Windows.Controls.TextBlock _emptyValsErrorTextBlock;
        private System.Windows.Controls.TextBlock _progressTextBlock;
        private System.Windows.Controls.Primitives.Popup _progressPopup;
        private List<VETRO_API> _api_requests;
        public VETRO_IntegrationWindow()
        {
            InitializeComponent();
            _api_requests = new List<VETRO_API>();
            _mapLayersComboBox = FindName("mapLayersCombo") as System.Windows.Controls.ComboBox;
            _apiTokenTextBox = FindName("apiToken") as System.Windows.Controls.TextBox;
            _apiSubmitButton = FindName("apiSubmit") as System.Windows.Controls.Button;
            _emptyValsErrorTextBlock = FindName("emptyValsError") as System.Windows.Controls.TextBlock;
            _progressPopup = FindName("progressPopup") as System.Windows.Controls.Primitives.Popup;
            _progressTextBlock = FindName("progressPopupText") as System.Windows.Controls.TextBlock;

            var map = MapView.Active.Map;
            if (map == null)
            {
                Debug.WriteLine("No active map found.");
                return;
            }
            // Iterate through all layers in the map
            foreach (var layer in map.GetLayersAsFlattenedList())
            {
                // Check if the layer is a FeatureLayer
                if (layer is FeatureLayer featureLayer)
                {
                    // Access the name of the FeatureLayer
                    //string layerName = layer.Name;
                    // Add items to the ComboBox
                    _mapLayersComboBox.Items.Add(layer);
                }
            }
        }

        public async void submitButtonOnClick(object sender, RoutedEventArgs e) 
        {
            VETRO_API apiInit = null;
            int maxFeatures = 500;
            if (_mapLayersComboBox.SelectedItem.ToString() == "" || _apiTokenTextBox.Text == "") { _emptyValsErrorTextBlock.Visibility = Visibility.Visible; }
            else {
                //display progress window
                _progressPopup.IsOpen = true;
                

                long requestStartOID = 1;
                long requestEndOID = 0;     
                FeatureLayer layerToPush = _mapLayersComboBox.SelectedItem as FeatureLayer;
                string geometry_type = "NULL";
                await QueuedTask.Run(() =>
                {
                    using (ArcGIS.Core.Data.Table shp_table = layerToPush.GetTable())
                    {
                        using (RowCursor rowCursor = shp_table.Search())
                        {
                        NewRequest:
                            apiInit = new VETRO_API();
                            this.Dispatcher.Invoke(() =>
                            {
                                apiInit._t = _apiTokenTextBox.Text;
                            });
                            string endString = "";
                            string errorMessage = "";
                            int requestFeaturesCount = 0;
                            bool reachedMaximumRequestSize = false;
                            apiInit._body = "{\r\n    \"features\": [";
                            while (rowCursor.MoveNext())
                            {
                                using (ArcGIS.Core.Data.Row row = rowCursor.Current)
                                {
                                    //set start oid
                                    if (row.GetObjectID() == requestEndOID + 1)
                                    {
                                        requestStartOID = row.GetObjectID();
                                        apiInit.setStartOID(requestStartOID);
                                    }

                                    //get geometry type
                                    if (row["Shape"] != null && row["Shape"].ToString() != " " && geometry_type == "NULL")
                                    {
                                        geometry_type = row["Shape"].ToString();
                                    }
                                    //TODO: Check that plan is Temp Plan

                                    //get coordinates (dependent on geometry type)
                                    //Point
                                    ArcGIS.Core.Data.Feature feature = row as ArcGIS.Core.Data.Feature;
                                    if (geometry_type == "ArcGIS.Core.Geometry.MapPoint")
                                    {
                                        apiInit.ClearBody();
                                        ArcGIS.Core.Geometry.MapPoint currentPoint = feature.GetShape() as ArcGIS.Core.Geometry.MapPoint;
                                        string pointX = currentPoint.X.ToString();
                                        string pointY = currentPoint.Y.ToString();
                                        string requestPointAddition = "\r\n     {\r\n        \"type\": \"Feature\",\r\n        \"geometry\": {\"type\":\"Point\", \"coordinates\":[" + pointX + ", " + pointY + "]},\r\n";
                                        apiInit._body += requestPointAddition;
                                        
                                        endString = this.FeatureStringFinisher(row);
                                        apiInit._body += endString;
                                        //this._api_init._body = this._api_init._body.TrimEnd(',');
                                        //this._api_init._body += "\r\n  ]\r\n}";
                                        requestFeaturesCount++;
                                    }
                                    //Line
                                    else if (geometry_type == "ArcGIS.Core.Geometry.Polyline")
                                    {
                                        ArcGIS.Core.Geometry.Polyline currentLine = feature.GetShape() as ArcGIS.Core.Geometry.Polyline;
                                        ReadOnlyPartCollection lineParts = currentLine.Parts;
                                        foreach (ReadOnlySegmentCollection segments in lineParts)
                                        {
                                            foreach (ArcGIS.Core.Geometry.Segment segment in segments)
                                            {
                                                string requestLineAddition = "\r\n     {\r\n        \"type\": \"Feature\",\r\n        \"geometry\": {\"type\":\"LineString\", \"coordinates\":[[" + segment.StartPoint.X.ToString() + ", " + segment.StartPoint.Y.ToString() + "], [" + segment.EndPoint.X.ToString() + ", " + segment.EndPoint.Y.ToString() + "]]},\r\n";
                                                apiInit._body += requestLineAddition;
                                                endString = this.FeatureStringFinisher(row);
                                                apiInit._body += endString;
                                            }
                                        }
                                    }
                                    //Polygon
                                    else if (geometry_type == "ArcGIS.Core.Geometry.Polygon")
                                    {
                                        ArcGIS.Core.Geometry.Polygon currentPolygon = feature.GetShape() as ArcGIS.Core.Geometry.Polygon;
                                        ReadOnlyPartCollection polygonParts = currentPolygon.Parts;
                                        int partCount = polygonParts.Count;
                                        int exteriorRingCount = currentPolygon.ExteriorRingCount;
                                        //we should only have one part and ring for each polygon layer because we don't have any polygon data that has parent/child relationships
                                        if (exteriorRingCount == 0 || partCount == 0)
                                        {
                                            //handle no exterior rings
                                        }
                                        if (exteriorRingCount > 1 || partCount > 1)
                                        {
                                            //handle multiple exterior rings
                                        }
                                        ReadOnlyPointCollection points = currentPolygon.Points;
                                        int pointCount = points.Count;
                                        string requestPolygonAddition = "\r\n    {\r\n        \"type\": \"Feature\",\r\n        \"geometry\": {\"type\":\"Polygon\", \"coordinates\":[[";
                                        apiInit._body += requestPolygonAddition;
                                        for (int i = 0; i < pointCount; i++)
                                        {
                                            requestPolygonAddition = "[" + System.String.Format("{0:0.00000000}", points[i].X) + ", " + System.String.Format("{0:0.00000000}", points[i].Y) + "],";
                                            apiInit._body += requestPolygonAddition;
                                            requestFeaturesCount++;
                                        }
                                        apiInit._body = apiInit._body.TrimEnd(',');
                                        apiInit._body += "]]},\r\n";
                                        endString = this.FeatureStringFinisher(row);
                                        apiInit._body += endString;
                                        //if (requestFeaturesCount >= maxFeatures)
                                        //{
                                        //    reachedMaximumRequestSize = true;
                                        //}
                                    }
                                    else
                                    {
                                        //handle geometry type not found
                                    }

                                    //end of row
                                    if (requestFeaturesCount >= maxFeatures)
                                    {
                                        requestEndOID = row.GetObjectID();
                                        apiInit.setEndOID(requestEndOID);
                                        apiInit._body = apiInit._body.TrimEnd(',');
                                        apiInit._body += "\r\n  ]\r\n}";

                                        if (_api_requests.Count % 9 == 0 && _api_requests.Count > 0)
                                        {
                                            Debug.WriteLine("Delaying for 10 seconds.");
                                            Thread.Sleep(11000);
                                        }
                                        _api_requests.Add(apiInit);
                                        _api_requests[_api_requests.Count - 1].CreateFeatures();
                                        //_progressTextBlock.Text += "\nAwaiting server response...";
                                        //errorMessage = apiInit.getResponse();
                                        //if (errorMessage != "None")
                                        //{
                                        //    _progressTextBlock.Text += "Records with OIDs between " + requestStartOID + " and " + requestEndOID + " failed to send.\n";
                                        //    _progressTextBlock.Text += errorMessage;
                                        //}

                                        goto NewRequest;
                                    }
                                }
                            }

                            //end of table reached

                            Debug.WriteLine("Last record reached");
                            apiInit._body = apiInit._body.TrimEnd(',');
                            apiInit._body += "\r\n  ]\r\n}";
                            requestEndOID = shp_table.GetCount();
                            apiInit.setEndOID(requestEndOID);
                            _api_requests.Add(apiInit);
                            _api_requests[_api_requests.Count - 1].CreateFeatures();
                            this.Dispatcher.Invoke(() =>
                            {
                                _progressTextBlock.Text += "\nAwaiting server response...";
                            });
                            bool nullResponseFound = true;
                            while (nullResponseFound)
                            {
                                foreach (VETRO_API apiCall in _api_requests) 
                                {
                                    if (apiCall.getResponse() == "") 
                                    {
                                        nullResponseFound = true;
                                        break;
                                    }
                                }
                                nullResponseFound = false;
                            }
                            foreach (VETRO_API apiCall in _api_requests)
                            {
                                errorMessage = apiCall.getResponse();
                                if (errorMessage != "None")
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        _progressTextBlock.Text += "\nOIDs " + apiCall.getStartOID() + " through " + apiCall.getEndOID() + " failed to send.";
                                        _progressTextBlock.Text += "\n" + errorMessage;
                                    });
                                }
                            } 
                        }
                    }
                });
                _progressTextBlock.Text += "\nPushing complete.";
                _progressPopup.StaysOpen = false;
                //this.Close();
            }
        }
        public string FeatureStringFinisher(ArcGIS.Core.Data.Row thisRow) {
            string returnString = "";

            //TODO: complete Dictionary of required properties for each importable layer
            Dictionary<string, string[]> VETRO_LayerRequiredAttributes = new Dictionary<string, string[]>();
            VETRO_LayerRequiredAttributes.Add("Micro Duct Bundle", ["Micro_Duct_Count"]);
            VETRO_LayerRequiredAttributes.Add("Project Area", ["Note", "Neighborhood_Name", "Census_Number", "Build_Type"]);

            //get x-vetro properties
            string requestAddition = "        \"x-vetro\": {\r\n            \"layer_id\": " + thisRow["v_layer_id"].ToString() + ",\r\n            \"plan_id\": " + thisRow["v_plan_id"].ToString() + "\r\n        },\r\n";
            returnString += requestAddition;

            //get properties based on layer required properties
            string[] requiredProperties = [];
            if (VETRO_LayerRequiredAttributes.ContainsKey(thisRow["v_layer"].ToString()))
            {
                requiredProperties = VETRO_LayerRequiredAttributes[thisRow["v_layer"].ToString()];
            }
            returnString += "        \"properties\": {";
            foreach (string property in requiredProperties)
            {
                requestAddition = "\r\n            \"" + property.Replace("_", " ") + "\": \"" + thisRow[property].ToString() + "\",";
                returnString += requestAddition;
            }
            returnString = returnString.TrimEnd(',');
            returnString += "\r\n        }\r\n    },";
            return returnString;
        }
    }
}
