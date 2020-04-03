<div align="center">
    <img align="center" src="https://s1.ax1x.com/2020/04/03/GUMZjS.png" alt="logo" width="200">
    <h1 align="center">Pixeval</h1>
    <p align="center">A Strong, Fast and Flexible Pixiv Client based on .NET Core and WPF</p>
    <p align="center">
        <img src="https://img.shields.io/github/stars/Rinacm/Pixeval?color=red&style=flat-square">
        <a href="mailto:decem0730@hotmail.com">
            <img src="https://img.shields.io/static/v1?label=contact%20me&message=hotmail&color=green&style=flat-square">
        </a>
        <a href="https://jq.qq.com/?_wv=1027&k=5hGmJbQ" target="_blank">
            <img src="https://img.shields.io/static/v1?label=chatting&message=qq&color=blue&style=flat-square"
        </a>
        <a href="http://47.95.218.243/index/index.html" target="_blank">
            <img src="https://img.shields.io/static/v1?label=homepage&message=pixeval&color=blueviolet&style=flat-square">
        </a>
        <a href="https://github.com/Rinacm/Pixeval/blob/master/LICENSE" target="_blank">
            <img src="https://img.shields.io/github/license/Rinacm/Pixeval?style=flat-square">
        </a>
        <a href="https://github.com/Rinacm/Pixeval/issues/new/choose" target="_blank">
            <img src="https://img.shields.io/static/v1?label=feedback&message=issues&color=pink&style=flat-square">
        </a>
        <a href="https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.3-windows-x64-installer" target="_blank">
            <img src="https://img.shields.io/static/v1?label=runtime&message=.NET%20Core%203.1&color=yellow&style=flat-square">
        </a>
    </p>
</div>

**更新：**
* 2020/1/9
    - 将用户浏览器和作品浏览器集成到主窗口中
    - 修改了图片列表的UI
    - 压缩包内包含了完整的.NET Core运行环境，避免有些非常非常可爱的萌新不看说明不知道要装.net，缺点是体积大大增加
* 2020/2/14
    - 优化了代码，删除了部分无用函数和工具类
* 2020/3/13
    - 现在下载时可以查看进度了
    - 你可以即时预览GIF了
    - 新增了使用SauceNAO作为源的搜图功能
    - 改进了核心API，优化了异常处理
    - 优化了图片浏览弹窗的开启速度
    - 现在可以在设置里选择不使用直连了</br>
* 2020/3/14
    - 标签趋势
    - 密码不再使用明文管理，而是通过Windows Credential Manager保存
* 2020/3/15
    - 添加了自动更新功能(尚处于测试)
    - 添加了一个导航到介绍网址的超链接
* 2020/3/29
    - 添加了缓存系统
    - 搜图功能可以拖拽文件了
    - 添加了自动下载Runtime的C++脚本


**BUG修复：**
* 2020/1/8
    - 修复了作者名包含不合法字符时导致的下载失败问题
* 2020/1/10
    - 修复了特辑下载时无法下载图集的问题
    - 修复了刚刚启动时无法下载图片/将图片添加到下载列表的问题
    - 优化了作者名包含不合法字符时的解决方案
* 2020/2/14
    - 修复了查看特辑时作品信息显示不正确的bug
    - 修复了查看某些作品时上传日期显示0001/01/01的bug
    - 修复了点击缩略图时可能无法打开大图的bug
    - 都0202年了，我依然没有女朋友，这也是个bug
* 2020/3/13
    - 修复了有时图片浏览底端按钮显示不全的问题
    - 修复了屏幕分辨率较低时无法显示完整侧边栏的问题
    - 修复了下载时容易出现空文件夹的问题</br>
* 2020/3/15
    - 修复了点击tag没有反应的bug
* 2020/3/21
    - 修复了排序选项和搜索起始页不起作用的BUG
* 2020/4/3
    - 优化了适配
  
**推荐：**
* 如果你希望找到一个Android上的免代理客户端，那么我推荐[@Notsfsssf](https://github.com/Notsfsssf)的作品[Pix-EzViewer](https://github.com/Notsfsssf/Pix-EzViewer) (本项目的免代理实现也源于此)
* 如果你更习惯于使用UWP，那么我建议你尝试由[@tobiichiamane](https://github.com/tobiichiamane)开发的[pixivuwp](https://github.com/tobiichiamane/pixivfs-uwp)

**注意：**
* 自版本1.4.0开始取消了自带的.NET Core 3.0 Runtime，因为有部分用户反馈体积太大，所以在使用之前请确保自己安装了.NET Core 3.0 Runtime（如果你选择下载[.NET Core Runtime 3.0.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)则需要注意同时下载Desktop Runtime和.NET Core Runtime,如果你下载的是3.1+的版本，则可以选择只下载Desktop Runtime）
* 自版本1.4.0开始不再使用单文件方式发布，由于单文件发布会导致一些奇怪的问题
* 自版本1.4.0开始可以选择是否开启直连了，默认关闭，如果自己需要直连请在设置中手动开启
* 自版本1.7.1起添加了runtime-installation.exe作为下载.NET Core Runtime的脚本，可以直接双击下载并安装

**下载：**
前往[Release页面](https://github.com/Rinacm/Pixeval/releases)下载最新版本的Release或者在项目[主页](http://47.95.218.243/index/index.html)下载

**如有遇到任何问题/有任何建议请通过以下方式联系作者：**
* 提交[issue](https://github.com/Rinacm/Pixeval/issues/new)
* 向decem0730@hotmail.com发送邮件
* 添加qq群815791942进行反馈

**鸣谢：(排名不分先后)**
* [@Notsfsssf](https://github.com/Notsfsssf)
* [@ControlNet](https://github.com/ControlNet)
* [@wulunshijian](https://github.com/wulunshijian)
* [@duiweiya](https://github.com/duiweiya)
* [@Lasm_Gratel](https://github.com/NanamiArihara)
* [@TheRealKamisama](https://github.com/TheRealKamisama)
* [@Summpot](https://github.com/Summpot)

**支持作者：**
如果你感觉该项目帮助到了你，欢迎前往[爱发电](https://afdian.net/@dylech30th)赞助我，你的支持是我维护项目的动力，谢谢！
