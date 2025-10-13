public static class EffectFactory
{
  public static IEffect GetEffect(EffectType effectType)
  {
    return effectType switch
    {
      EffectType.None => throw new InvalidOperationException("No effect associated with EffectType.None"),
      EffectType.Boring => new BoringEffect(),
      EffectType.Magnetic => new MagneticEffect(),
      EffectType.Exploding => new ExplodingEffect()
    };
  }
}
