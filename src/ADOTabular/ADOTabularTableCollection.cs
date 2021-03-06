﻿using System.Collections.Generic;
using System.Collections;
//using Microsoft.AnalysisServices.AdomdClient;

namespace ADOTabular
{
    public class ADOTabularTableCollection:IEnumerable<ADOTabularTable>
    {
        
        private readonly IADOTabularConnection _adoTabConn;
        private readonly ADOTabularModel  _model;
        private SortedDictionary<string, ADOTabularTable> _tables;
        private object mutex = new object();

        public ADOTabularTableCollection(IADOTabularConnection adoTabConn, ADOTabularModel model)
        {
            _adoTabConn = adoTabConn;
            _model = model;

        }

        private SortedDictionary<string,ADOTabularTable> InternalTableCollection
        {
            get
            {
                if (_tables == null)
                {
                    lock (mutex)
                    {
                        if (_tables == null)
                        {
                            _adoTabConn.Visitor.Visit(this);
                        }
                    }
                }
                return _tables;
            }
        }

        public ADOTabularModel Model
        {
            get { return _model; }
        }

        public int Count
        {
            get { return InternalTableCollection.Count; }
        }

        public void Add(ADOTabularTable table)
        {
            if (_tables == null)
            {
                _tables = new SortedDictionary<string, ADOTabularTable>();
            }
            _tables.Add(table.Name, table);
        }

        public ADOTabularTable this[string index]
        {
            get
            {
                return InternalTableCollection[index];
            }
        }

        public bool ContainsKey(string index)
        {
            return InternalTableCollection.ContainsKey(index);
        }

        public ADOTabularTable this[int index]
        {
            get
            {
                string[] keys = new string[InternalTableCollection.Count];
                InternalTableCollection.Keys.CopyTo(keys, 0);
                return InternalTableCollection[keys[index]];
            }

        }


        
        public ADOTabularTable GetById(string internalId)
        {
            foreach (var t in InternalTableCollection.Values)
            {
                if (t.InternalReference == internalId)
                {
                    return t;
                }
            }
            return null;
        }

        public IEnumerator<ADOTabularTable> GetEnumerator()
        {
            foreach (var t in InternalTableCollection.Values)
            {
                yield return t;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsCached { get { return _tables != null; } }
    }
}

