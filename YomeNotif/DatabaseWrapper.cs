using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;

namespace YomeNotif
{
    // 基本的にココでxmlをいじいじする
    public class DatabaseWrapper
    {
        // マイドキュメントを取得しておく
        string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public DatabaseWrapper()
        { 
            // xmlの存在を確認する。存在したなら処理は終わりだ。
            if (File.Exists(directoryPath+@"\YomeNotif\YomeDB.xml"))
                return;
            // マイドキュメントにYomeNotifディレクトリが存在しなければ作っておく
            if (!Directory.Exists(directoryPath + @"\YomeNotif"))
                Directory.CreateDirectory(directoryPath+@"\YomeNotif");
            // xmmlファイルを作成。初期値に熊野を入れておく。
            var XDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),
                new XElement("YomeDB",
                    new XElement("yome",
                        new XElement("ID", "1"),
                        new XElement("Name", "熊野"),
                        new XElement("Image", "kumano.jpg"),
                        new XElement("Voices", ""),
                        new XElement("Text", "")
                        )));
            // xmlファイルの書き込み。
            XDoc.Save(directoryPath + @"\YomeNotif\YomeDB.xml");
        }

        // 艦娘リストの追加処理。引数で名前とトップ画像のパスを貰ってくる
        public void WriteYomeList(string name, string image)
        {
            // お約束
            XDocument xdoc = XDocument.Load(directoryPath + @"\YomeNotif\YomeDB.xml");
            XElement xele = xdoc.Element("YomeDB");
            int i = xele.Elements("yome").Count() + 1;

            // ぶち込む
            xele.Add(
                new XElement("yome",
                    new XElement("ID", i),
                        new XElement("Name", name),
                        new XElement("Image", image),
                        new XElement("Voices", ""),
                        new XElement("Text", "")
                        ));
            // 保存
            xdoc.Save(directoryPath + @"\YomeNotif\YomeDB.xml");
        }

        // 艦娘リストの削除処理。引数で名前を貰ってくる
        public void DeleteYomeList(string name)
        {
            // お約束
            XDocument xdoc = XDocument.Load(directoryPath + @"\YomeNotif\YomeDB.xml");
            XElement xele = xdoc.Element("YomeDB");

            // 名前で当該データを検索する
            XElement ship = (from item in xele.Elements("yome")
                             where item.Element("Name").Value == name
                             select item).Single();
            // 離婚
            ship.Remove();
            xdoc.Save(directoryPath + @"\YomeNotif\YomeDB.xml");
        }

        // 艦娘リストの編集処理。引数で元の名前、変更後の名前、トップ画像を貰ってくる
        public void UpdateYomeList(string defaultName, string name, string image)
        {
            XDocument xdoc = XDocument.Load(directoryPath + @"\YomeNotif\YomeDB.xml");
            XElement xele = xdoc.Element("YomeDB");
            // 登録されている名前で検索
            XElement ship = (from item in xele.Elements("yome")
                             where item.Element("Name").Value == defaultName
                             select item).Single();
            // Rewrite
            ship.Element("Name").Value = name;
            ship.Element("Image").Value = image;
            xdoc.Save(directoryPath + @"\YomeNotif\YomeDB.xml");
        }

        // 艦娘リストの追加時に重複を防止するためにリストを持っていく
        public string[] CheckYomeList()
        {
            XDocument xdoc = XDocument.Load(directoryPath + @"\YomeNotif\YomeDB.xml");
            XElement xele = xdoc.Element("YomeDB");

            IEnumerable<XElement> yome = xele.Elements("yome");

            int num = yome.Count();

            string[] data = new string[num];

            for (int i = 0; i < num; i++)
            {
                data[i] = yome.Elements("Name").ToArray()[i].Value;
            }

            return data;
        }

        // ListBoxの削除・追加時に、元の位置に戻すためにIDを取得
        public int GetYomeID(string Name)
        {
            XDocument xdoc = XDocument.Load(directoryPath + @"\YomeNotif\YomeDB.xml");
            XElement xele = xdoc.Element("YomeDB");
            XElement ship = (from item in xele.Elements("yome")
                             where item.Element("Name").Value == Name
                             select item).Single();
            int number = int.Parse(ship.Element("ID").Value);
            return number;
        }

        // ListBox構成のために艦娘リストを取得
        public string[,] GetYomeList()
        {
            XDocument xdoc = XDocument.Load(directoryPath + @"\YomeNotif\YomeDB.xml");
            XElement xele = xdoc.Element("YomeDB");

            IEnumerable<XElement> yome = xele.Elements("yome");

            int num = yome.Count();

            string[,] data = new string[num, 2];

            for (int i = 0; i < num; i++)
            {
                data[i, 0] = yome.Elements("Name").ToArray()[i].Value;
                data[i, 1] = yome.Elements("Image").ToArray()[i].Value;
            }
            
            return data;
        }

        // 時報データ設定のために、時報データ全般を返す
        public string[,] GetNotifList(string Name)
        {
            XDocument xdoc = XDocument.Load(directoryPath + @"\YomeNotif\YomeDB.xml");
            XElement xele = xdoc.Element("YomeDB");
            XElement ship = (from item in xele.Elements("yome")
                             where item.Element("Name").Value == Name
                             select item).Single();
            string Voice = ship.Element("Voices").Value;
            string Text = ship.Element("Text").Value;

            string[] Voices;
            string[] Texts;

            string[,] data = new string[24, 2];

            if (Voice != "" && Text != "")
            {
                Voices = Voice.Split(',');
                Texts = Text.Split(',');

                for (int i = 0; i < 24; i++)
                {
                    data[i, 0] = Voices[i];
                    data[i, 1] = Texts[i];
                }
            }
            else
            {
                for (int i = 0; i < 24; i++)
                {
                    data[i, 0] = "";
                    data[i, 1] = "";
                } 
            }

            return data;
        }

        // 時報データの設定。一括で毎度上書きする。
        public bool SetYomeList(string name, string[,] data)
        {
            try
            {
                XDocument xdoc = XDocument.Load(directoryPath + @"\YomeNotif\YomeDB.xml");
                XElement xele = xdoc.Element("YomeDB");
                XElement ship = (from item in xele.Elements("yome")
                                 where item.Element("Name").Value == name
                                 select item).Single();
                string[] Voices = new string[24];
                string[] Text = new string[24];

                for (int i = 0; i < 24; i++)
                {
                    Voices[i] = data[i, 0];
                    Text[i] = data[i, 1];
                }

                ship.Element("Voices").Value = string.Join(",", Voices);
                ship.Element("Text").Value = string.Join(",", Text);
                xdoc.Save(directoryPath + @"\YomeNotif\YomeDB.xml");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
