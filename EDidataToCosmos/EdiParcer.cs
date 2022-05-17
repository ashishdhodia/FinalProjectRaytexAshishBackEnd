using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDIdataToCosmos
{
    public class EdiParcer
    {
        public string parseEDI()
        {
            string textjsonModel = System.IO.File.ReadAllText(@"D:\intership\frontend\FinalProjectRaytex\BE\eModalMicroservices\EDidataToCosmos\C-SharpModel.json");
            dynamic jsonModel = JsonConvert.DeserializeObject(textjsonModel);
            //Console.WriteLine(jsonModel);


            string text = System.IO.File.ReadAllText(@"D:\intership\frontend\FinalProjectRaytex\BE\eModalMicroservices\EDidataToCosmos\03_ShipmentStatus.txt");
            text = String.Concat(text.Where(c => !Char.IsWhiteSpace(c)));
            //Console.WriteLine(text);

            string txnModel = System.IO.File.ReadAllText(@"D:\intership\frontend\FinalProjectRaytex\BE\eModalMicroservices\EDidataToCosmos\txnModel.txt");
            //Console.WriteLine(text);

            dynamic lineData = text.Split("~");
            //Console.WriteLine(lineData.Length);

            string[] feeTypes = { "22", "4I", "4I1", "4I2", "4I3", "4I4", "4I5", "4I6", "4I7", "4I8", "4I9", "4IV", "4IE", "BBC", "DVF", "FCH", "GEN", "GEC", "HZP", "SCR", "USD", "WCR", "MSL" };


            string code = "";
            float amount = 0;

            for (int i = 0; i < lineData.Length; i++)
            {
                dynamic singleData = lineData[i].Split("*");
                if (singleData[0] == "ISA")
                {
                    for (int j = 0; j < (singleData.Length - 1); j++)
                    {
                        jsonModel["ISA"][$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                    }
                }
                else if (singleData[0] == "GS")
                {
                    for (int j = 0; j < (singleData.Length - 1); j++)
                    {
                        jsonModel["Groups"][0]["GS"][$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                    }
                }
                else if (singleData[0] == "ST")
                {
                    JObject nTxn = JObject.Parse(txnModel);
                    for (int j = 0; j < (singleData.Length - 1); j++)
                    {
                        nTxn["ST"][$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                    }

                    while (code != "SE")
                    {
                        singleData = lineData[i].Split("*");
                        //Console.WriteLine(singleData[0]);
                        code = singleData[0];
                        i++;
                        if (code == "SE")
                            i--;

                        if (singleData[0] == "B4")
                        {
                            for (int j = 0; j < (singleData.Length - 1); j++)
                            {
                                nTxn["B4"][$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                            }
                            nTxn["id"] = nTxn["B4"]["B47"].ToString() + nTxn["B4"]["B48"].ToString();
                        }
                        else if (singleData[0] == "N9")
                        {
                            JObject nNine = JObject.Parse(@"{
                                          'N91': '',
                                          'N92': '',
                                          'N93': '',
                                          'N94': ''
                                        }");
                            for (int j = 0; j < (singleData.Length - 1); j++)
                            {
                                nNine[$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                                if ($"{singleData[0]}{j + 1}".ToString() == "N91" && feeTypes.Contains($"{singleData[j + 1]}"))
                                {
                                    amount = float.Parse($"{singleData[j + 2].ToString()}") + amount;
                                }
                            }

                            
                            ((global::Newtonsoft.Json.Linq.JArray)(JArray)nTxn["N9Loop"]).Add(nNine);
                        }
                        else if (singleData[0] == "Q2")
                        {
                            for (int j = 0; j < (singleData.Length - 1); j++)
                            {
                                if (singleData[j + 1] == "")
                                {
                                    continue;
                                    //Console.WriteLine("HERE");
                                    nTxn["Q2"][$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                                }
                            }
                        }
                        else if (singleData[0] == "SG")
                        {
                            JObject SG = JObject.Parse(@"{
                                          'SG1': '',
                                          'SG2': '',
                                          'SG3': '',
                                          'SG4': '',
                                          'SG5': '',
                                          'SG6': '',
                                          'SG7': '',
                                        }");
                            for (int j = 0; j < (singleData.Length - 1); j++)
                            {
                                SG[$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                                //if (nTxn["id"].ToString() == "CSLU6172205")
                            }
                            ((global::Newtonsoft.Json.Linq.JArray)(JArray)nTxn["SG"]).Add(SG);
                            
                            //Console.WriteLine(nTxn["id"].ToString());
                        }
                        else if (singleData[0] == "SE")
                        {
                            for (int j = 0; j < (singleData.Length - 1); j++)
                            {
                                nTxn["SE"][$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                            }
                        }
                        else if (singleData[0] == "R4")
                        {
                            JObject R4 = JObject.Parse(@"{
                                  'R41': '',
                                  'R42': '',
                                  'R43': '',
                                  'R44': '',
                                  'R45': '',
                                  'R46': ''
                                }");
                            for (int j = 0; j < (singleData.Length - 1); j++)
                            {
                                R4[$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                            }
                            ((global::Newtonsoft.Json.Linq.JArray)(JArray)nTxn["R4Loop"]).Add(R4);

                        }
                        else if (singleData[0] == "DTM")
                        {
                            JObject DTM = JObject.Parse(@"{
                                  'DTM1': '',
                                  'DTM2': '',
                                  'DTM3': '',
                                  'DTM4': '',
                                  'DTM5': '',
                                  'DTM6': ''
                                }");
                            for (int j = 0; j < (singleData.Length - 1); j++)
                            {
                                DTM[$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                            }
                            ((global::Newtonsoft.Json.Linq.JArray)(JArray)nTxn["DTM"]).Add(DTM);
                        }
                        
                    }

                    //Console.WriteLine(amount + " " + nTxn["id"]);
                    //Console.WriteLine(nTxn["fees"]);
                    nTxn["fees"] = amount.ToString();
                    amount = 0;

                    code = "";

                    //Console.WriteLine("Here");

                    JArray itemNOne = (JArray)jsonModel["Groups"][0]["Transactions"];
                    itemNOne.Add(nTxn);
                }
                else if (singleData[0] == "GE")
                {
                    for (int j = 0; j < (singleData.Length - 1); j++)
                    {
                        jsonModel["Groups"][0]["GETrailers"][0][$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                    }
                }
                else if (singleData[0] == "IEA")
                {
                    for (int j = 0; j < (singleData.Length - 1); j++)
                    {
                        jsonModel["IEATrailers"][0][$"{singleData[0]}{j + 1}"] = singleData[j + 1];
                    }
                }

            }
            //Console.WriteLine(jsonModel);

            dynamic jsonString = JsonConvert.SerializeObject(jsonModel);

            string filePath = @"D:\intership\frontend\FinalProjectRaytex\BE\eModalMicroservices\EDidataToCosmos\Output.json";
            Console.WriteLine("Creating Json On Local Storage...");
            File.WriteAllText(filePath, jsonString);
            Console.WriteLine("Done");

            Console.WriteLine("");
            return jsonString;
        }
    }
}
