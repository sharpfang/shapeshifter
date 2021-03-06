﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public Shape StartingShape;
	public List<Shape> Shapes;

	[HideInInspector]
	public Shape CurrentShape;

	private CommonInputManager inputManager;

    private GameObject cameraReference;

	private Rigidbody rb;

	void Awake()
	{
		inputManager = CommonInputManager.instance;
        cameraReference = GameObject.FindWithTag("MainCamera");
		rb = GetComponent<Rigidbody>();
	}

	void Start()
	{
		// Disable shapes
		foreach (Shape shape in Shapes)
		{
			shape.gameObject.SetActive(false);
		}

		// Activate the starting shape
		CurrentShape = StartingShape;
		ShiftTo(StartingShape);
	}

	void Update()
	{
		if (inputManager.SwapInput)
		{
			ShiftTo(NextShape());
		}

		float dt = Time.deltaTime;

        Vector3 cameraForward = (transform.position - cameraReference.transform.position);
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = Quaternion.AngleAxis(90, Vector3.up) * cameraForward;

        rb.AddTorque(cameraForward * -inputManager.HorizontalInput * dt * CurrentShape.RotationSpeed);
        rb.AddTorque(cameraRight * inputManager.VerticalInput * dt * CurrentShape.RotationSpeed);

        rb.AddForce(cameraForward * inputManager.VerticalInput * dt * CurrentShape.ForwardForce);
        rb.AddForce(cameraRight * inputManager.HorizontalInput * dt * CurrentShape.ForwardForce);
    }

	Shape NextShape()
	{
		int index = Shapes.IndexOf(CurrentShape) + 1;
		if (index >= Shapes.Count) index = 0;
		return Shapes[index];
	}

	void ShiftTo(Shape toShape)
	{
		// Disable old shape and activate the new one
		CurrentShape.gameObject.SetActive(false);
		toShape.gameObject.SetActive(true);
		CurrentShape = toShape;

		// Reset orientation
		transform.rotation = Quaternion.Euler(new Vector3(
			0f,
			transform.rotation.y,
			0f
		));

		// Update rigidbody params
		rb.mass = toShape.Mass;
		rb.centerOfMass = toShape.CenterOfMass;
		rb.drag = toShape.Drag;
		rb.angularDrag = toShape.AngularDrag;

		// Jump
		if (toShape.JumpOnShift) rb.AddForce(toShape.JumpForce.x, toShape.JumpForce.y, toShape.JumpForce.z);
	}
}
