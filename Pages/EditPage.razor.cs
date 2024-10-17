using H.Tools.Data;
using KeyGenerator.Models;
using KeyGenerator.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Text;

namespace KeyGenerator.Pages;

public partial class EditPage
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!firstRender) return;

        if (Id != null)
        {
            EditApp = DataSourceService.Table<KeyApp>().Where(x => x.Id == Id).First();
            StateHasChanged();
        }
    }

    #region 编辑
    [Parameter]
    public int? Id { get; set; }

    private KeyApp EditApp { get; set; } = new();

    private async Task Update()
    {
        if (string.IsNullOrEmpty(EditApp.Name))
        {
            await DialogService.ShowErrorAsync("请输入名称");
            return;
        }
        if (string.IsNullOrEmpty(EditApp.Secret))
        {
            await DialogService.ShowErrorAsync("请输入密钥");
            return;
        }

        if (Id == null)
        {
            DataSourceService.Insert(EditApp);
            Id = (int)EditApp.Id;
            ToastService.ShowSuccess("已添加");
        }
        else
        {
            DataSourceService.Update(EditApp);
            ToastService.ShowSuccess("已更新");
        }
    }

    private async Task Delete()
    {
        var dialog = await DialogService.ShowConfirmationAsync("即将删除应用，该操作不可恢复，确认继续？", "确认", "取消", "确认删除");
        if ((await dialog.Result).Cancelled)
        {
            return;
        }

        DataSourceService.Delete(EditApp);
        await GoBack();
    }

    private async Task GoBack()
    {
        await JSRuntime.InvokeVoidAsync("history.go", -1);
    }
    #endregion

    #region 生成
    private string _key = string.Empty;
    private string _license = string.Empty;
    private DateTime? _startTime = DateTime.Today;
    private DateTime? _endTime = DateTime.Today.AddYears(1);

    private async void Generate()
    {
        var license = await CreateLicense();
        if (!string.IsNullOrEmpty(license))
        {
            await Clipboard.SetTextAsync(license);
            ToastService.ShowSuccess("已复制到剪切板");
        }
        _license = license ?? "";
    }

    private async Task<string?> CreateLicense()
    {
        if (_startTime == null)
        {
            await DialogService.ShowErrorAsync("请选择开始时间");
            return null;
        }
        if (_endTime == null)
        {
            await DialogService.ShowErrorAsync("请选择结束时间");
            return null;
        }
        if (string.IsNullOrEmpty(EditApp.Secret))
        {
            await DialogService.ShowErrorAsync("请输入密钥");
            return null;
        }
        if (string.IsNullOrEmpty(_key))
        {
            await DialogService.ShowErrorAsync("请输入Key");
            return null;
        }
        if (!string.IsNullOrEmpty(_key) && !Guid.TryParse(_key, out _))
        {
            await DialogService.ShowErrorAsync("Key格式不正确");
            return null;
        }

        var startTimeStr = _startTime.Value.ToString("yyyy-MM-dd");
        var endTimeStr = _endTime.Value.ToString("yyyy-MM-dd");
        var md5 = MD5.HashData(Encoding.UTF8.GetBytes(EditApp.Secret + startTimeStr + endTimeStr + _key)).ToX2();
        var str = string.Join(" ", md5, startTimeStr, endTimeStr, _key);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }
    #endregion
}
