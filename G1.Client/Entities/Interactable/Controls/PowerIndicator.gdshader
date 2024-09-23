shader_type canvas_item;

uniform bool is_active = true; 
uniform uint max_progress = 5;
uniform uint current_progress = 3;
uniform sampler2D indicatorTexture;

void fragment() {	
	vec4 color = texture(TEXTURE, UV);
	if(color.a == 0.){
		float level = float(max_progress - current_progress);
		float y = (UV.y - 0.05) * float(max_progress);
		if(y > level){
			float y = y - float(int(y));
			vec2 uv = vec2(UV.x, y);
			color = texture(indicatorTexture, uv);
		}
	}
	if(!is_active){
		color *= 0.8;
	}
	
	COLOR = color;
}
