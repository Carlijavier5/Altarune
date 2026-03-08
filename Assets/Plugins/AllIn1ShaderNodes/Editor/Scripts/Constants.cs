namespace AllIn1ShaderNodes
{
	public static class Constants
	{
		public const string LAST_RENDER_PIPELINE_CHECKED_KEY = "AllIn13DShader_LastRenderPipeline";
		public const string RENDER_PIPELINE_CHANGED_KEY = "ALlIn1_RenderPipelineChanged";
		public const string ON_POST_PROCESS_ALL_ASSETS_FIRST_TIME_KEY = "AllIn1_OnPostProcessAllAssetsFirstTime";

		//Main Assembly Name
		public const string MAIN_ASSEMBLY_NAME = "AllIn1ShaderNodes.asmdef";

		//Plugin Root Folder
		public const string PLUGIN_FOLDER = "Assets/Plugins/AllIn1ShaderNodes";

		//Effect Categories
		public const string PLUGIN_NAME					= "AllIn1ShaderNodes";
		public const string PLUGIN_NAME_WITH_SEPARATOR	= PLUGIN_NAME + " - ";

		public const string EFFECT_CATEGORY_UV					= PLUGIN_NAME_WITH_SEPARATOR + "UV Effects";
		public const string EFFECT_CATEGORY_VERTEX				= PLUGIN_NAME_WITH_SEPARATOR + "Vertex Effects";
		public const string EFFECT_CATEGORY_SPECULAR			= PLUGIN_NAME_WITH_SEPARATOR + "Specular Models";
		public const string EFFECT_CATEGORY_PROCEDURAL_SHAPES	= PLUGIN_NAME_WITH_SEPARATOR + "Procedural Shapes";
		public const string EFFECT_CATEGORY_GLOBAL_ILLUMINATION = PLUGIN_NAME_WITH_SEPARATOR + "Global Illumination";
		public const string EFFECT_CATEGORY_DIFFUSE				= PLUGIN_NAME_WITH_SEPARATOR + "Diffuse Models";
		public const string EFFECT_CATEGORY_DEPTH_EFFECTS		= PLUGIN_NAME_WITH_SEPARATOR + "Depth Effects";
		public const string EFFECT_CATEGORY_COLOR_EFFECTS		= PLUGIN_NAME_WITH_SEPARATOR + "Color Effects";
		public const string EFFECT_CATEGORY_ALPHA_EFFECTS		= PLUGIN_NAME_WITH_SEPARATOR + "Alpha Effects";
		public const string EFFECT_CATEGORY_SPRITES				= PLUGIN_NAME_WITH_SEPARATOR + "Sprites";
	}
}