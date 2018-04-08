using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace ContentManagement.Common.WebToolkit
{
    public class OpenSearchResult : ActionResult
    {
        public string ShortName { set; get; }
        public string Description { set; get; }
        public string SearchForm { set; get; }
        public string FavIconUrl { set; get; }
        public string SearchUrlTemplate { set; get; }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            writeToResponse(response);
        }

        private void writeToResponse(HttpResponse response)
        {
            var mediaType = new MediaTypeHeaderValue("application/opensearchdescription+xml");
            //mediaType.Encoding = Encoding.UTF8;
            response.ContentType = mediaType.ToString();
            using (var xmlWriter = XmlWriter.Create(response.Body, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                xmlWriter.WriteStartElement("OpenSearchDescription", "http://a9.com/-/spec/opensearch/1.1/");

                xmlWriter.WriteElementString("ShortName", ShortName);
                xmlWriter.WriteElementString("Description", Description);
                xmlWriter.WriteElementString("InputEncoding", "UTF-8");
                xmlWriter.WriteElementString("SearchForm", SearchForm);

                xmlWriter.WriteStartElement("Url");
                xmlWriter.WriteAttributeString("type", "text/html");
                xmlWriter.WriteAttributeString("template", SearchUrlTemplate);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Image");
                xmlWriter.WriteAttributeString("width", "16");
                xmlWriter.WriteAttributeString("height", "16");
                xmlWriter.WriteString(FavIconUrl);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
                xmlWriter.Close();
            }
        }
    }
}
