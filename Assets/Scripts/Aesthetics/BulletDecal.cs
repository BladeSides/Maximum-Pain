using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BulletDecal : MonoBehaviour
{
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private float _timer = 0f;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color StartingColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartingColor = _spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        _spriteRenderer.color = Color.Lerp(StartingColor, new Color(0, 0, 0, 0), _timer / _lifetime);
        _timer += Time.deltaTime;
        if (_timer > _lifetime)
        {
            Destroy(transform.root.gameObject); //Destroy root because the sprite renderer is placed a little further to the gameobject so Z-Fighting doesn't occur
        }
    }
}
