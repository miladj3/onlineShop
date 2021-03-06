﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using onlineShop.Contracts;
using onlineShop.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace onlineShop.Services
{
    public class BreadcrumbNavBuilder : IBreadcrumbNavBuilder
    {
        private const string bnbMapPath = "/breadcrumb_sitemap.xml";

        private readonly IHostingEnvironment _env;

        public BreadcrumbNavBuilder(IHostingEnvironment env)
        {
            _env = env;
        }

        public void CreateForNode(string currentNodeName, object BNBData, Controller controller)
        {
            var breadcrumbItemList = new List<BreadcrumbNavItem>();

            var path = _env.ContentRootPath + bnbMapPath;

            // read sitemap
            XDocument xmlDoc = XDocument.Load(path);
            XElement element = xmlDoc.Descendants("node").FirstOrDefault(n => n.Attribute("name").Value == currentNodeName);

            if (element != null)
            {
                // get current node and all its ancestors
                var nodeTree = element.AncestorsAndSelf().Where(n => n.Name == "node").ToList();

                foreach (var node in nodeTree)
                {
                    // extract each node data from sitemap
                    var nodeName = node.Attribute("name").Value;
                    var actionName = node.Attribute("action").Value;
                    var controllerName = node.Attribute("controller").Value;
                    var srcRouteId = node.Attribute("src_route_id").Value;
                    var srcName = node.Attribute("src_name").Value;
                    var defaultName = node.Attribute("default_name").Value;
                    var routeIdPropName = node.Attribute("route_id_name").Value;

                    // try to extract value for route id and name to be displayed
                    var routeIdValue = TryExtractPropertyValue(srcRouteId, BNBData);
                    var extractedName = TryExtractPropertyValue(srcName, BNBData);
                    var itemName = String.IsNullOrEmpty(extractedName) ? defaultName : extractedName;

                    // use expando object to collect route parameters
                    dynamic exp = new ExpandoObject();
                    var expDict = exp as IDictionary<String, object>;
                    expDict.Add(routeIdPropName, routeIdValue);

                    // generate absolute path
                    var uh = new UrlHelper(controller.ControllerContext);
                    var itemPath = uh.Action(actionName, controllerName, (object)exp);

                    // append to the list of breadcrumb navigation items
                    breadcrumbItemList.Add(new BreadcrumbNavItem()
                    {
                        IsActive = String.Equals(nodeName, currentNodeName),
                        ItemDisplayName = itemName,
                        ItemUrl = itemPath
                    });
                }
            }

            breadcrumbItemList.Reverse();

            // serialized and send to tempdata
            var breadcrumbItemListSerialized = JsonConvert.SerializeObject(breadcrumbItemList);
            controller.TempData["BNBData"] = breadcrumbItemListSerialized;
        }

        private string TryExtractPropertyValue(string propertyName, object obj)
        {
            try
            {
                var type = obj.GetType();
                PropertyInfo propInfo = type.GetProperty(propertyName);
                object propValue = propInfo.GetValue(obj, null);
                var propValueString = propValue.ToString();

                return propValueString;
            }
            catch
            {
                return null;
            }
        }
    }
}
