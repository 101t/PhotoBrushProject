using System;
using System.Collections.Generic;
using System.Text;

namespace Painting.Shapes
{
    [Serializable]
    public class GDIProperties
    {
        List<string> m_propertyName;
        List<object> m_propertyValue;
        List<string> m_description;

        public GDIProperties(int capacity)
        {
            m_propertyName = new List<string>(capacity);
            m_propertyValue = new List<object>(capacity);
            m_description = new List<string>(capacity);
        }

        public int PropertyCount
        {
            get { return m_propertyName.Count; }
        }

        public void AddProperty(string name, object value, string description)
        {
            m_propertyName.Add(name);
            m_propertyValue.Add(value);
            m_description.Add(description);
        }

        public string GetPropertyName(int index)
        {
            if (index != -1 && index < (m_propertyName.Count))
                return m_propertyName[index];

            throw new Exception("Property index out of bounds!");
        }

        public string GetDescription(int index)
        {
            if (index != -1 && index < (m_propertyName.Count))
                return m_description[index];

            throw new Exception("Property index out of bounds!");
        }

        public int GetIndex(string propertyName)
        {
            for (short i = 0; i < m_propertyName.Count; i++)
            {
                if (propertyName == m_propertyName[i])
                {
                    return i;
                }
            }
            //property was not found
            throw new Exception("Property: " + propertyName + " does not exist!");
        }

        public object GetValue(string propertyName)
        {
            for (short i = 0; i < m_propertyName.Count; i++)
            {
                if (propertyName == m_propertyName[i])
                {
                    return m_propertyValue[i];
                }
            }
            //property was not found
            throw new Exception("Property: " + propertyName + " does not exist!");
        }

        public object GetValue(int index)
        {
            if (index != -1 && index < (m_propertyName.Count))
                return m_propertyValue[index];

            throw new Exception("Property index out of bounds!");
        }

        public void SetValue(string propertyName, object value)
        {
            for (short i = 0; i < m_propertyName.Count;i++)
            {
                if (propertyName == m_propertyName[i])
                {
                    m_propertyValue[i] = value;
                }
            }
            //property was not found
            throw new Exception("Property: "+propertyName+ " does not exist!");
        }

        public void SetValue(int index, object value)
        {
            if (index != -1 && index < (m_propertyName.Count))
                m_propertyValue[index] = value;
            else
                throw new Exception("Property index out of bounds!");
        }

    }
}
