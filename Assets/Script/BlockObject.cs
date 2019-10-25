using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlockObject : MonoBehaviour
{

	public Block block;
	public int dist;
	public GameObject Highlighted;
	public GameObject ColorBlindImage;
	public BlockStatus blockStatus;

	void Start()
	{
		Debug.Log("========");
		GetComponent<BoxCollider2D>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = true;
	}

	private void BlockClicked()
	{
		GameManager.Instance.TouchBlock(block);
	}

	public void SetBlock()
	{
		Highlighted.SetActive(false);

		this.gameObject.GetComponent<SpriteRenderer>().color = ViewManager.Instance.colorPalette [(int)block.blockColor];
		ColorBlindImage.GetComponent<SpriteRenderer>().sprite = ViewManager.Instance.ColorBlindImage [(int)block.blockColor];

		blockStatus = BlockStatus.idle;
	}

	public IEnumerator BlockPop(int t)
	{
		blockStatus = BlockStatus.destroyed;
		Highlighted.SetActive(false);
		yield return new WaitForSeconds(ViewManager.Instance.burstInterval * t);
		GameManager.Instance.PlaySound();
		Vector2 OriginScale = this.transform.localScale;
		float timer = 0;
		while (timer < ViewManager.Instance.burstTime) {
			float x = (timer / ViewManager.Instance.burstTime);
			//Debug.Log(x + " " + (1 - x) * (Mathf.Pow(x, 2) + x + 1));
			this.transform.localScale = Vector2.Lerp(OriginScale, Vector2.zero, 1 - Mathf.Pow(1 - x, 2));
			yield return new WaitForSeconds(Time.deltaTime);
			timer += Time.deltaTime;
		}

		Highlighted.SetActive(false);
		this.transform.SetParent(ViewManager.Instance.BlockContainer.transform);
		this.gameObject.SetActive(false);

		yield return null;
	}

	public IEnumerator BlockMove(Vector2 dest)
	{
		blockStatus = BlockStatus.moving;
		Highlighted.SetActive(false);
		yield return new WaitForSeconds(ViewManager.Instance.burstDelay * ViewManager.Instance.burstInterval + ViewManager.Instance.burstTime);
		Vector2 OriginPosition = this.transform.localPosition;
		float timer = 0;
		while (timer < ViewManager.Instance.movingTimeToDest) {
			this.transform.localPosition = Vector2.Lerp(OriginPosition, dest, 1 - Mathf.Pow(1 - (timer / ViewManager.Instance.movingTimeToDest), 2));
			yield return new WaitForSeconds(Time.deltaTime);
			timer += Time.deltaTime;
		}

		this.transform.localPosition = dest;
		blockStatus = BlockStatus.idle;
	}

	public IEnumerator BlockSpawn(int t)
	{
		blockStatus = BlockStatus.moving;
		Vector2 OriginScale = this.transform.localScale;
		this.transform.localScale = Vector2.zero;
		yield return new WaitForSeconds((t - 2) * ViewManager.Instance.spawningInterval);
		float timer = 0;
		while (timer < ViewManager.Instance.spawningTime) {
			float x = (timer / ViewManager.Instance.spawningTime);
			this.transform.localScale = Vector2.Lerp(Vector2.zero, OriginScale, 1 - Mathf.Pow(1 - x, 2));
			yield return new WaitForSeconds(Time.deltaTime);
			timer += Time.deltaTime;
		}

		this.transform.localScale = OriginScale;
		blockStatus = BlockStatus.idle;
	}

	void OnMouseOver()
	{
		Debug.Log("Mouse is over GameObject.");
		if (!Highlighted.activeSelf && blockStatus == BlockStatus.idle && Input.mousePosition != ViewManager.Instance.prevMousePosition) {
			ViewManager.Instance.ActiveHighlighted(block);
		}
		if (Input.GetMouseButtonUp(0) && blockStatus == BlockStatus.idle && block.blockColor != BlockColor.G) {
			BlockClicked();
		}
	}

	void OnMouseExit()
	{
		Debug.Log("Mouse is exit GameObject.");
		if (blockStatus == BlockStatus.idle)
			ViewManager.Instance.DeactiveHighlighted();
	}
}

public enum BlockStatus
{
	idle,
	moving,
	destroyed

}
