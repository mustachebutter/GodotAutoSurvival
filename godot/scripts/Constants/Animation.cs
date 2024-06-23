using Godot;

public static class AnimationCreator
{
    public enum AnimationTypes
    {
        UI_DamageNumber,
    }
    public static Animation CreateAnimation(AnimationTypes animationType)
    {
        Animation animation = new Animation();

        switch (animationType)
        {
            case AnimationTypes.UI_DamageNumber:
                
                int trackIndexPosition = animation.AddTrack(Animation.TrackType.Value);
                animation.TrackSetPath(trackIndexPosition, ".:position");
                
                animation.TrackInsertKey(trackIndexPosition, 0.0f, new Vector2(0.0f, 0.0f));
                animation.TrackInsertKey(trackIndexPosition, 1.0f, new Vector2(0.0f, -100f));

                int trackIndexOpacity = animation.AddTrack(Animation.TrackType.Value);
                animation.TrackSetPath(trackIndexOpacity, "Label:modulate");
                animation.TrackInsertKey(trackIndexOpacity, 0.0f, new Color(1.0f, 1.0f, 1.0f, 0.0f));
                animation.TrackInsertKey(trackIndexOpacity, 0.2f, new Color(1.0f, 1.0f, 1.0f, 1.0f));
                animation.TrackInsertKey(trackIndexOpacity, 0.8f, new Color(1.0f, 1.0f, 1.0f, 1.0f));
                animation.TrackInsertKey(trackIndexOpacity, 1.0f, new Color(1.0f, 1.0f, 1.0f, 0.0f));

                animation.LoopMode = Animation.LoopModeEnum.None;
                break;
            default:
                break;
        }

        return animation;
    }
}