shader_type canvas_item;

uniform bool is_active = true;
uniform uint max_progress = 5;
uniform uint current_progress = 3;
uniform vec4 ActiveColor: source_color=vec4(0.4, 0.6, 0.9, 1.0);
uniform vec4 NotActiveColor: source_color=vec4(0.4, 0.3, 0.9, 1.0);

uniform float border_size = 0.05;

void fragment() {
	vec4 color;
	if(is_active){
		color = ActiveColor;
	}else{
		color = NotActiveColor;
	}
	
	if(UV.x<border_size || UV.x> 1. - border_size || UV.y> 1. - border_size ){
	
		COLOR = color;
	}
}
