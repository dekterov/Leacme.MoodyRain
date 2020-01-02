// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();
	private AudioStream rainAudio = GD.Load<AudioStream>("res://assets/rain.ogg");

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		var env = GetNode<WorldEnvironment>("sky").Environment;
		env.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		env.BackgroundMode = Godot.Environment.BGMode.Sky;
		env.BackgroundSky = new PanoramaSky() { Panorama = ((Texture)GD.Load("res://assets/night_city.hdr")) };
		env.BackgroundSkyRotationDegrees = new Vector3(0, 0, 0);
		env.BackgroundSkyCustomFov = 130;

		InitSound();
		AddChild(Audio);

		rainAudio.Play(Audio);
		Audio.Seek((float)GD.RandRange(0, rainAudio.GetLength()));

		var glass = GetNode("Window").GetNode<MeshInstance>("Glass");
		var noiseTexture = new NoiseTexture() {
			Noise = new OpenSimplexNoise() {
				Period = 40,
				Persistence = 0.2f
			},
			AsNormalmap = true,
			BumpStrength = 32
		};
		glass.MaterialOverride = new SpatialMaterial() {
			AlbedoColor = new Color(1, 1, 1, 0.05f),
			AlbedoTexture = noiseTexture,
			NormalEnabled = true,
			NormalScale = -0.1f,
			NormalTexture = noiseTexture,
			RefractionEnabled = true,
			RefractionScale = 0.1f,
			RefractionTexture = noiseTexture
		};

		var rainWindowParticles = new CPUParticles();
		AddChild(rainWindowParticles);
		rainWindowParticles.RotateX(Mathf.Deg2Rad(16));
		rainWindowParticles.Mesh = new SphereMesh() { Radius = 0.005f, Height = 0.01f, RadialSegments = 4, Rings = 4 };
		rainWindowParticles.Amount = 500;
		rainWindowParticles.EmissionShape = CPUParticles.EmissionShapeEnum.Sphere;
		rainWindowParticles.EmissionSphereRadius = 4.5f;

		var rainParticles = new CPUParticles();
		AddChild(rainParticles);
		rainParticles.RotateX(Mathf.Deg2Rad(16));
		rainParticles.Translate(new Vector3(7.6f, 2.7f, 0));
		rainParticles.Mesh = new SphereMesh() { Radius = 0.005f, Height = 1, RadialSegments = 4, Rings = 1 };
		rainParticles.Amount = 200;
		rainParticles.EmissionShape = CPUParticles.EmissionShapeEnum.Sphere;
		rainParticles.EmissionSphereRadius = 8f;

	}

	public override void _Process(float delta) {

	}

}
