using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(BulgeVignettePPRenderer), PostProcessEvent.AfterStack, "Custom/BulgeVignette")]
public sealed class BulgeVignettePP : PostProcessEffectSettings
{
    public TextureParameter noise = new TextureParameter();
    public ColorParameter color = new ColorParameter();

    public FloatParameter threshold = new FloatParameter() { value = 0.2f };
    public FloatParameter smoothness = new FloatParameter() { value = 0.2f };
    public FloatParameter tiling = new FloatParameter() { value = 1f };
    public FloatParameter scroll = new FloatParameter() { value = 0f };
}

public sealed class BulgeVignettePPRenderer : PostProcessEffectRenderer<BulgeVignettePP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/CustomVignette"));
        sheet.properties.SetColor("_Color", settings.color);
        sheet.properties.SetFloat("_Threshold", settings.threshold);
        sheet.properties.SetFloat("_Smoothness", settings.smoothness);
        sheet.properties.SetFloat("_Tiling", settings.tiling);
        sheet.properties.SetFloat("_Scroll", settings.scroll);
        if(settings.noise != null)
            sheet.properties.SetTexture("_NoiseTex", settings.noise);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}