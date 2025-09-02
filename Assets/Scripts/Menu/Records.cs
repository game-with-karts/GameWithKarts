using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using GWK.Util;

public class RecordsMenu : MonoBehaviour {
    [SerializeField] private TMP_Text recordTimeText;

    void OnEnable() {
        SetRecordText("Hill");
    }

    private List<TimeRecord> records;
    public List<TimeRecord> Records {
        get {
            string recordData = PlayerPrefs.GetString("Records");
            if (recordData == string.Empty) {
                records = new();
                return records;
            }
            records = JsonUtility.FromJson<Records>(recordData).records;
            return records;
        }
    }

    public void SetRecordText(string level) {
        TimeRecord rec = Records.Where(r => r.track == level).SingleOrDefault();
        if (rec is null) {
            recordTimeText.text = "--:--.---";
            return;
        }
        recordTimeText.text = StringsUtil.GetFormattedTime(rec.time);
    }
    
}
