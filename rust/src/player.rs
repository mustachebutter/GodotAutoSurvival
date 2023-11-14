use std::str::FromStr;
use godot::engine::{AnimationTree, AnimationNodeStateMachinePlayback};
use godot::prelude::*;

#[derive(GodotClass)]
#[class(base=Node2D)]
struct Player 
{
    health: f64,
    #[base]
    node2d: Base<Node2D>,
}

use godot::{engine::Node2DVirtual, prelude::Node2D};

#[godot_api]
impl Node2DVirtual for Player 
{
    fn init(node2d: Base<Node2D>) -> Self 
    {
        godot_print!("Created Player node!");
        Self 
        { 
            health: 100.0,
            node2d
        }
    }

    fn process(&mut self, delta: f64) 
    {
        let input = Input::singleton();
        let mut velocity = Vector2::new(0.0, 0.0);

        if Input::is_action_pressed(&input, StringName::from_str("Up").unwrap())
        {
            velocity.y -= 1.0;
        }
        if Input::is_action_pressed(&input, StringName::from_str("Left").unwrap())
        {
            velocity.x -= 1.0;
        }
        if Input::is_action_pressed(&input, StringName::from_str("Down").unwrap())
        {
            velocity.y += 1.0;
        }
        if Input::is_action_pressed(&input, StringName::from_str("Right").unwrap())
        {
            velocity.x += 1.0;
        }


        let anim_tree_path = match NodePath::from_str("CharacterBody2D/AnimationTree")
        {
            Ok(node) => node,
            Err(err) => {
                godot_error!("Failed to parse node from path: {:?}", err);
                return;
            }
        };
        let mut animation_tree = self.node2d.get_node(anim_tree_path)
            .expect("Anim Tree node not found");

        let playback_variant = animation_tree.get(StringName::from_str("parameters/playback").unwrap());

        let playback: Result<AnimationNodeStateMachinePlayback, VariantConversionError> = playback_variant.try_to();
        
        match playback {
            Ok(mut playback) => 
            {
                if velocity == Vector2::ZERO
                {
                    playback.travel(StringName::from_str("Idle").unwrap());
                }
                else
                {
                    playback.travel(StringName::from_str("Walk").unwrap());
                    animation_tree
                        .set(StringName::from_str("parameters/Idle/blend_position").unwrap(), velocity.to_variant());
        
                    animation_tree
                        .set(StringName::from_str("parameters/Walk/blend_position").unwrap(), velocity.to_variant());
                }        
            },
            Err(err) =>
            {
                godot_error!("Failed to convert playback from variant");
            }
        }


        let pos = self.node2d.get_position();
        self.node2d.set_position(pos + velocity)
    }
}