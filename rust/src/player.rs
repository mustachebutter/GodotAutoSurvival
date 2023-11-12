use std::str::FromStr;
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

        let pos = self.node2d.get_position();
        self.node2d.set_position(pos + velocity)
    }
}