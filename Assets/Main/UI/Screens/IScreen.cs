using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public interface IScreen
{
    void Initialize();
    VisualElement Root { get; }
}
