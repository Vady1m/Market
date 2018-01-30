using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace market
{
    public struct MainData
    {
        public double Price;
        public int Count;
        public int BuyCountOpt;
        public int BuyCountClient;
    }

    public partial class Form1 : Form
    {
        string FilePath = null;
        List<string> TypeGoods = new List<string>();
        List<string> TypeGoodsWithoutCount = new List<string>();

        List<MainData> ListMD = new List<MainData>();

        // количество строк в файле, количество разных типов товаров
        int CountStroki;

        string MainOutput;
        double MainProfit;
        double PriceAllOpt;

        bool blg = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "d:\\";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MainOutput = "";
                button2.Enabled = true;
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                            FilePath = openFileDialog1.FileName;
                            openFileDialog1.Reset();                   
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }

                myStream.Close();
                

                MainOutput = MainOutput + "ИНФОРМАЦИЯ ОБ ОССОРТИМЕНТЕ ТОВАРА" + "\n" + "\n";

                StreamReader sr = new StreamReader(FilePath, Encoding.GetEncoding(1251));
                MainOutput = MainOutput + sr.ReadToEnd();
                sr.Close();

                StreamReader sr1 = new StreamReader(FilePath, Encoding.GetEncoding(1251));
                CountStroki = 0;
                bool bl = true;
                

                while (bl)
                {
                    string str1 = sr1.ReadLine();
                    string strwhc = str1;
                    int j = 0;

                    if (str1 != null)
                    {
                        while (str1[str1.Length - j - 1] != ' ')
                            j++;
                        strwhc = strwhc.Remove(str1.Length - j - 2);
                    }

                    if (str1 != null) { TypeGoods.Add(str1); TypeGoodsWithoutCount.Add(strwhc); CountStroki++; }
                    else bl = false;
                }

                sr1.Close();

                string str3, str4;
                bool bl1, bl2;
                int i;
                

                for (int n = 0; n < CountStroki; n++)
                {
                    MainData MD;
                    str3 = TypeGoods[n];
                    str4 = null;
                    bl1 = true;
                    bl2 = false;
                    i = 1;

                    while (bl1)
                    {
                        if (str3[i] == ',') { bl2 = true; i++; }
                        while (bl2)
                        {
                            if (str3[i + 1] != ',') { str4 = str4 + str3[i + 1]; }
                            if (str3[i + 1] == ',') { bl1 = false; bl2 = false; }
                            i++;
                        }
                        i++;
                    }

                    MD.Price = Convert.ToDouble(str4);

                    str4 = null;
                    i = 1;
                    int Lth = str3.Length;

                    while (str3[Lth - i] != ' ')
                    {
                        str4 = str3[Lth - i] + str4;
                        i++;
                    }

                    MD.Count = Convert.ToInt32(str4);
                    MD.BuyCountOpt = 0;
                    MD.BuyCountClient = 0;

                    ListMD.Add(MD);

                }

                blg = true;
            }

            richTextBox1.Text = MainOutput;

            for (int cstr = 0; cstr < CountStroki; cstr++)
            {
                MainData MD1;
                MD1 = ListMD[cstr];
                MD1.BuyCountOpt = 0;
                MD1.BuyCountClient = 0;
                ListMD[cstr] = MD1;
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            if (blg)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                MainOutput = MainOutput + "\n" + "\n";
                MainOutput = MainOutput + "ЭМУЛЯЦИЯ РАБОТЫ МАГАЗИНА\n";

                MainProfit = 0;
                PriceAllOpt = 0;

                for (int day = 1; day <= 30; day++)
                {
                    MainOutput = MainOutput + "\n" + "ДЕНЬ " + day.ToString() + "\n";

                    Random rnd = new Random();
                    int CountClient;
                    int CountPrices;

                    //один робочий день, 13 часов
                    for (int i = 0; i < 13; i++)
                    {
                        int time = i + 8;
                        CountClient = rnd.Next(1, 10);

                        //количество клиентов за 1 час
                        for (int j = 0; j < CountClient; j++)
                        {
                            CountPrices = rnd.Next(0, 10);
                            MainOutput = MainOutput + "\n" + "Время " + time.ToString() + "-" + (time + 1).ToString();
                            if (CountPrices != 0) MainOutput = MainOutput + " покупатель " + (j + 1).ToString() + "\n";
                            else MainOutput = MainOutput + " покупатель " + (j + 1).ToString() + "\n" + "ничего не купил \n";

                            double procent;

                            procent = 1.1;
                            if (day % 7 == 6 || day % 7 == 0) procent = 1.15;
                            if (time == 18 || time == 19) procent = 1.08;
                            if (CountPrices >= 2) procent = 1.07;

                            //покупка товаров клиентом
                            for (int k = 0; k < CountPrices; k++)
                            {
                                int TypeNumber = rnd.Next(0, CountStroki);

                                if (ListMD[TypeNumber].Count > 0)
                                {
                                    // присвоение значения
                                    MainData MD;
                                    MD = ListMD[TypeNumber];
                                    MD.Count = MD.Count - 1;
                                    MD.BuyCountClient = MD.BuyCountClient + 1;
                                    ListMD[TypeNumber] = MD;

                                    double PriceForClient = Math.Round((ListMD[TypeNumber].Price * procent), 2);
                                    MainProfit = MainProfit + (PriceForClient - ListMD[TypeNumber].Price);

                                    MainOutput = MainOutput + "купил " + TypeGoodsWithoutCount[TypeNumber] +
                                        " цена продажи " + PriceForClient.ToString() + " наценка " + (100 * procent - 100).ToString() + "%\n";
                                }
                                else MainOutput = MainOutput + "товара нет в наличии " + TypeGoodsWithoutCount[TypeNumber] + "\n";

                            }
                        }
                    }

                    for (int counts = 0; counts < CountStroki; counts++)
                    {
                        MainData MD;
                        MD = ListMD[counts];
                        if (MD.Count < 10)
                        {
                            MD.Count = MD.Count + 150;
                            MD.BuyCountOpt = MD.BuyCountOpt + 150;
                        }
                        ListMD[counts] = MD;

                    }
                }

                richTextBox1.Text = MainOutput;

                string FilePathT = FilePath;
                int FilePathTi = 0;
                while (FilePath[FilePath.Length - 1 - FilePathTi] != '.')
                {
                    FilePathTi++;
                }

                FilePathT = FilePathT.Remove(FilePath.Length - FilePathTi);
                FilePathT = FilePathT + "txt";

                string strT = null;
                string strCSV = null;



                for (int strTi = 0; strTi < CountStroki; strTi++)
                {
                    strT = strT + TypeGoodsWithoutCount[strTi] + " продано "
                        + ListMD[strTi].BuyCountClient + " дозакуплено " + ListMD[strTi].BuyCountOpt + "\r\n";
                    PriceAllOpt = PriceAllOpt + ListMD[strTi].BuyCountOpt * ListMD[strTi].Price;

                    if (strTi != CountStroki - 1) strCSV = strCSV + TypeGoodsWithoutCount[strTi] + ", " + (ListMD[strTi].Count).ToString() + "\r\n";
                    else strCSV = strCSV + TypeGoodsWithoutCount[strTi] + ", " + (ListMD[strTi].Count).ToString();
                }

                strT = strT + "\r\n" + "прибыль мазагина от продаж " + (Math.Round(MainProfit, 2)).ToString() + "\r\n";
                strT = strT + "затраченные средства на дозакупку товара " + PriceAllOpt.ToString();

                // Create the file.
                using (FileStream fsT = File.Create(FilePathT))
                {
                    Byte[] info;
                    info = new UTF8Encoding(false).GetBytes(strT);
                    // Add some information to the file.
                    fsT.Write(info, 0, info.Length);
                    fsT.Close();

                }

                StreamWriter output = new StreamWriter(FilePath, false, Encoding.GetEncoding(1251));
                output.WriteLine(strCSV);
                output.Close();

                button1.Enabled = true;
            
            } else MessageBox.Show("Загрузите файл");
        }
    }
}
