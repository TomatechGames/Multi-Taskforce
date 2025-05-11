shader_type spatial;
uniform sampler2D tex : source_color;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	COLOR = texture(tex, vec2(0,UV.y));
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
