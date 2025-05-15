using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;
using ArcGIS.Desktop.Internal.Mapping.Raster.RasterHistogram;
using ArcGIS.Desktop.Internal.Catalog.PropertyPages.NetworkDataset;
using ArcGIS.Core.Data.UtilityNetwork.Trace;
using System.Security.Cryptography;
using System.Windows;

class VETRO_API
{
    public string _body = "";
    public string _t = "";
    public string _responseError = "";
    public long _startOID = 0;
    public long _endOID = 0;
    private string _base_url = "https://api.vetro.io/v3/";
    public VETRO_API() { 
        
    }

    public string getResponse() 
    {
        return this._responseError;
    }

    public void setResponse(string r)
    {
        this._responseError = r;
    }
    public long getStartOID()
    {
        return this._startOID;
    }

    public void setStartOID(long id)
    {
        this._startOID = id;
    }
    public long getEndOID()
    {
        return this._endOID;
    }

    public void setEndOID(long id)
    {
        this._endOID = id;
    }

    public void ClearBody() {
        _body = "";
    }
    public async void CreateFeatures() 
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vetro.io/v3/features");
        // request.Headers.Add("content-type", "application/json")
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("token", _t);
        var content = new StringContent(_body, null, "application/json");
        request.Content = content;
        Debug.WriteLine("Attempting Request...");
        var response = await client.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
            _responseError = "None";
        }
        catch (HttpRequestException e)
        {
            _responseError = e.Message;
            Debug.WriteLine(_body);
            Debug.WriteLine(e);
        }
    }
}
