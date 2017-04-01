using UnityEngine;
using System.Collections;

namespace DI.VisualScripting{
	public interface IVisualScripting {
		DIRootComponent[] rootComponents { set; get; }
		MonoBehaviour getMono { get; }
		string visualScriptingName { get; }
	}
	
}
