using System;

public class FixedParametersManager
{
    public FixedParametersManager()
    {
        Instance=this;
    }

     enum ReplaceOptions
    {
        matchCase = 0,
        matchWholeWord = true,
        matchWildCards = false,
        matchSoundsLike = false,
        matchAllWordForms = false,
        forward = true,
        format = false,
        matchKashida = false,
        matchDiacritics = false,
        matchAlefHamza = false,
        matchControl = false,
             read_only = false,
           visible = true,
    replace = 2,
            wrap = 1
    }
}
