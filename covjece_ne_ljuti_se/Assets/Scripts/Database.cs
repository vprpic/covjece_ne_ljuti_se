using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Db4objects.Db4o;

public class Database{

	public IObjectContainer dbContainer;

	public Database()
	{
		dbContainer = CreateDatabase();
	}

	public IObjectContainer CreateDatabase()
	{
		return Db4oFactory.OpenFile("C:\\Users\\maggi\\Documents\\covjece_ne_ljuti_se\\covjece_ne_ljuti_se\\Assets\\db\\tempdb.yap");
	}

}
