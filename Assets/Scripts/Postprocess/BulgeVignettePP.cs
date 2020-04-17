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
    public FloatParameter distort = new FloatParameter() { value = 0.2f };
    public FloatParameter distortSub = new FloatParameter() { value = 0.2f };
    public FloatParameter smoothness = new FloatParameter() { value = 0.2f };
    public Vector4Parameter tiling = new Vector4Parameter() { value = new Vector4(1,1,1,1) };
    public Vector4Parameter tiling2 = new Vector4Parameter() { value = new Vector4(1,1,1,1) };
    public FloatParameter scroll = new FloatParameter() { value = 0f };
}

public sealed class BulgeVignettePPRenderer : PostProcessEffectRenderer<BulgeVignettePP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/BulgeVignette"));
        sheet.properties.SetColor("_Color", settings.color);
        sheet.properties.SetFloat("_Threshold", settings.threshold);
        sheet.properties.SetFloat("_Distort", settings.distort);
        sheet.properties.SetFloat("_DistortSub", settings.distortSub);
        sheet.properties.SetFloat("_Smoothness", settings.smoothness);
        sheet.properties.SetVector("_Tiling", settings.tiling);
        sheet.properties.SetVector("_Tiling2", settings.tiling2);
        sheet.properties.SetFloat("_Scroll", settings.scroll);
        if(settings.noise != null)
            sheet.properties.SetTexture("_NoiseTex", settings.noise);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}