using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;
using System.Text.RegularExpressions;
namespace Slimes
{
    public class GrowthStages
    {
		public List<GrowthStage> growthStages;
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
			Log.Message("LoadDataFromXmlCustom: " + xmlRoot.ChildNodes.Count);
			growthStages = new List<GrowthStage>(new GrowthStage[xmlRoot.ChildNodes.Count]);
			var regex = new Regex("([0-9]+)$");
			foreach (XmlNode childNode in xmlRoot.ChildNodes)
			{
				if (!(childNode is XmlComment))
				{
					var growthStage = new GrowthStage();
					foreach (XmlNode childNode2 in childNode.ChildNodes)
					{
						if (!(childNode2 is XmlComment))
						{

							if (childNode2.Name == "massStage")
                            {
								growthStage.massStage = float.Parse(childNode2.InnerText);
							}
							else if (childNode2.Name == "drawSize")
							{
								growthStage.drawSize = float.Parse(childNode2.InnerText);
							}
							else if (childNode2.Name == "color")
							{
								growthStage.color = ParseHelper.ParseColor(childNode2.InnerText);
							}
							else if (childNode2.Name == "graphicPath")
							{
								growthStage.graphicPath = childNode2.InnerText;
							}
							else if (childNode2.Name == "drawSize")
							{
								growthStage.drawSize = float.Parse(childNode2.InnerText);
							}
							else if (childNode2.Name == "massConversionEfficiency")
							{
								growthStage.massConversionEfficiency = float.Parse(childNode2.InnerText);
							}
							else if (childNode2.Name == "massConversionRate")
							{
								growthStage.massConversionRate = float.Parse(childNode2.InnerText);
							}
							else if (childNode2.Name == "baseHungerRate")
							{
								growthStage.baseHungerRate = float.Parse(childNode2.InnerText);
							}
						}
					}
					var index = int.Parse(regex.Match(childNode.Name).Groups[1].Value) - 1;
					growthStages[index] = growthStage;
					Log.Message("index: " + index + " - " + growthStages[index]?.graphicPath);
				}
			}

			for (var ind = 0; ind < growthStages.Count; ind++)
            {
				Log.Message("1: " + ind + " - " + growthStages[ind].massStage);
            }
			Log.Message("END");
		}
    }
}
