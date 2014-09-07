using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace PhotoBrushProject
{
    class FileSerializer
    {
        public static void Serialize<T>(string Path, T Collection)
        {
            try
            {
                using (FileStream FS = new FileStream(Path, FileMode.OpenOrCreate))
                {
                    BinaryFormatter BF = new BinaryFormatter();
                    BF.Serialize(FS, Collection);
                    FS.Close();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "Error File Serializer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static T Deserialize<T>(string Path) where T : new()
        {
            try
            {
                T Collection = new T();
                using (FileStream FS = new FileStream(Path, FileMode.Open))
                {
                    BinaryFormatter BF = new BinaryFormatter();
                    Collection = ((T)BF.Deserialize(FS));
                    FS.Close();
                }
                return Collection;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "Error File Serializer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new T();
            }
        }
    }
}
