using System.Text;

public class ChatMarkerFormatter
{
    private char spellMarker = '\u0001';
    private string openTag;
    private const string closeTag = "</color>";
    private StringBuilder sBuilder;

    public const int maxCharCapacity = 50;
    private int maxBuilderCapacity = 256;

    public ChatMarkerFormatter()
    {
        openTag = Utils.ColorToTag(UIColors.Instance.SpellHighlightColor);
        sBuilder = new StringBuilder(maxBuilderCapacity);
        sBuilder.Clear();
    }

    public char SpellMarker => spellMarker;

    #region métodos
    // Reemplaza todos los spellmarkers por cadenas vacías
    public string Strip(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        sBuilder.Clear();

        foreach (char c in s)
        {
            if (c != spellMarker)
            {
                sBuilder.Append(c);
            }
        }
        return sBuilder.ToString();
    }

    // Envuelve una cadena con los marcadores de color
    public string WrapInSpellMarker(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        sBuilder.Clear();
        sBuilder.Append(spellMarker).Append(s).Append(spellMarker);
        return sBuilder.ToString();
    }

    /// <summary>
    /// Método para pasar los marcadores a texto con color una vez recibido 
    /// el mensaje en local, fuera de cualquier NetworkVariable.
    /// </summary>
    /// <param name="s">cadena de entrada con marcadores a poner con color</param>
    /// <returns></returns>
    public string ToRich(string s)
    {
        if (string.IsNullOrEmpty(s) || s.IndexOf(spellMarker) < 0) return s;

        sBuilder.Clear();
        bool open = false;
        int count = 0;

        foreach (char c in s) if (c == spellMarker) count++;

        if (count % 2 == 1)
        {
            sBuilder.Append(openTag);
            open = true;
        }

        foreach (char c in s)
        {
            if (c == spellMarker)
            {
                if (open)
                {
                    // Si ya hay algún marcador, cerrarlo
                    sBuilder.Append(closeTag);
                    open = false;
                }
                else
                {
                    // Poner un marcador e indicar que está abierto
                    sBuilder.Append(openTag);
                    open = true;
                }
            }
            else sBuilder.Append(c);
        }
        if (open) sBuilder.Append(closeTag);
        return sBuilder.ToString();
    }
    #endregion
}