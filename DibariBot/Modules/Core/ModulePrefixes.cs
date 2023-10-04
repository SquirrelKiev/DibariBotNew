namespace DibariBot.Modules;

public static class ModulePrefixes
{
    #region Misc

    public const string RED_BUTTON = "rb:";

    public const string MANGA_MODULE_PREFIX = "m:";

    #endregion

    #region MangaDex Search

    public const string MANGADEX_SEARCH_BUTTON_PREFIX = "mdsb:";
    public const string MANGADEX_SEARCH_DROPDOWN_PREFIX = "mdsd:";

    #endregion

    #region Config - Core

    public const string CONFIG_PAGE_SELECT_PAGE = "c-s:";
    public const string CONFIG_PAGE_SELECT_PAGE_BUTTON = "c-sb:";

    #endregion

    #region Config - Default Manga

    public const string CONFIG_DEFAULT_MANGA = "c-dm";

    public const string CONFIG_DEFAULT_MANGA_SET = $"{CONFIG_DEFAULT_MANGA}s:";
    public const string CONFIG_DEFAULT_MANGA_SET_MANGA_INPUT = $"{CONFIG_DEFAULT_MANGA_SET}manga-url:";
    public const string CONFIG_DEFAULT_MANGA_SET_MODAL = $"{CONFIG_DEFAULT_MANGA_SET}modal";
    public const string CONFIG_DEFAULT_MANGA_SET_CHANNEL_INPUT = $"{CONFIG_DEFAULT_MANGA_SET}channel:";
    public const string CONFIG_DEFAULT_MANGA_SET_SUBMIT_BUTTON = $"{CONFIG_DEFAULT_MANGA_SET}sub:";

    public const string CONFIG_DEFAULT_MANGA_REMOVE = $"{CONFIG_DEFAULT_MANGA}r:";
    public const string CONFIG_DEFAULT_MANGA_REMOVE_DROPDOWN = $"{CONFIG_DEFAULT_MANGA_REMOVE}dd";

    #endregion

    #region Config - Regex Filters

    public const string CONFIG_FILTERS = "c-f";

    public const string CONFIG_FILTERS_CONFIRMATION = $"{CONFIG_FILTERS}-c";
    public const string CONFIG_FILTERS_CONFIRMATION_FILTER_TYPE = $"{CONFIG_FILTERS_CONFIRMATION}-ft:";
    public const string CONFIG_FILTERS_CONFIRMATION_ADD_BUTTON = $"{CONFIG_FILTERS_CONFIRMATION}-a:";
    public const string CONFIG_FILTERS_CONFIRMATION_CHANNEL_SELECT = $"{CONFIG_FILTERS_CONFIRMATION}-cs:";
    public const string CONFIG_FILTERS_CONFIRMATION_CHANNEL_SCOPE = $"{CONFIG_FILTERS_CONFIRMATION}-cs2:";

    public const string CONFIG_FILTERS_ADD_BASE = $"{CONFIG_FILTERS}a";
    public const string CONFIG_FILTERS_OPEN_MODAL_BUTTON = $"{CONFIG_FILTERS_ADD_BASE}:";

    public const string CONFIG_FILTERS_MODAL_BASE = $"{CONFIG_FILTERS}-m";
    public const string CONFIG_FILTERS_MODAL = $"{CONFIG_FILTERS_MODAL_BASE}:";
    public const string CONFIG_FILTERS_MODAL_FILTER_TEXTBOX = $"{CONFIG_FILTERS_MODAL_BASE}-rtb";
    public const string CONFIG_FILTERS_MODAL_TEMPLATE_TEXTBOX = $"{CONFIG_FILTERS_MODAL_BASE}-ttb";

    public const string CONFIG_FILTERS_REMOVE_BASE = $"{CONFIG_FILTERS}r";
    public const string CONFIG_FILTERS_REMOVE_BUTTON = $"{CONFIG_FILTERS_REMOVE_BASE}:";
    public const string CONFIG_FILTERS_REMOVE_FILTER_SELECT = $"{CONFIG_FILTERS_REMOVE_BASE}-s:";
    
    public const string CONFIG_FILTERS_EDIT_BASE = $"{CONFIG_FILTERS}e";
    public const string CONFIG_FILTERS_EDIT_BUTTON = $"{CONFIG_FILTERS_EDIT_BASE}:";
    public const string CONFIG_FILTERS_EDIT_FILTER_SELECT = $"{CONFIG_FILTERS_EDIT_BASE}-s:";

    #endregion

    #region Config - Prefix

    public const string CONFIG_PREFIX_BASE = "c-p";
    public const string CONFIG_PREFIX_MODAL = $"{CONFIG_PREFIX_BASE}-m";
    public const string CONFIG_PREFIX_MODAL_PREFIX_TEXTBOX = $"{CONFIG_PREFIX_BASE}-ptb";
    public const string CONFIG_PREFIX_MODAL_BUTTON = $"{CONFIG_PREFIX_MODAL}-b";

    #endregion
}
