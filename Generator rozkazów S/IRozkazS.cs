using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S;

public interface IRozkazS
{
    void Update_Time();
    int Number { get; set; }
    User? Isedr { set; }
    User? FromOrder { set; }
    IList<Station>? Stations { set; }
    string Post { set; }
    DateOnly Date { get; }
    Station Station { get; }
    void TriggerValidation();
    bool Validate();
}