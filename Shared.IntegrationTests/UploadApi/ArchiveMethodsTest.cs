﻿using System;
using System.Collections.Generic;
using CloudinaryDotNet.Actions;
using NUnit.Framework;

namespace CloudinaryDotNet.IntegrationTest.UploadApi
{
    public class ArchiveMethodsTest : IntegrationTestBase
    {
        [Test]
        public void TestCreateArchive()
        {
            var targetPublicId = GetUniquePublicId();
            var archiveTag = GetMethodTag();

            var res = UploadResourceForTestArchive<ImageUploadParams>(archiveTag, true, 2.0);

            var parameters = new ArchiveParams()
                                            .Tags(new List<string> { archiveTag, "no_such_tag" })
                                            .TargetPublicId(targetPublicId)
                                            .TargetTags(new List<string> { m_apiTag });
            var result = m_cloudinary.CreateArchive(parameters);
            Assert.AreEqual($"{targetPublicId}.{FILE_FORMAT_ZIP}", result.PublicId);
            Assert.AreEqual(1, result.FileCount);

            var res2 = UploadResourceForTestArchive<ImageUploadParams>(archiveTag, false, 500);

            var transformations = new List<Transformation> { m_simpleTransformation, m_updateTransformation };
            parameters = new ArchiveParams().PublicIds(new List<string> { res.PublicId, res2.PublicId })
                                            .Transformations(transformations)
                                            .FlattenFolders(true)
                                            .SkipTransformationName(true)
                                            .UseOriginalFilename(true)
                                            .Tags(new List<string> { archiveTag })
                                            .TargetTags(new List<string> { m_apiTag });
            result = m_cloudinary.CreateArchive(parameters);
            Assert.AreEqual(2, result.FileCount);
        }

        [Test]
        public void TestCreateArchiveRawResources()
        {
            var raw = Api.GetCloudinaryParam(ResourceType.Raw);
            var tag = GetMethodTag();

            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(m_testImagePath),
                Folder = "test_folder",
                Type = STORAGE_TYPE_PRIVATE,
                Tags = $"{tag},{m_apiTag}"
            };
            var uploadResult1 = m_cloudinary.Upload(uploadParams, raw);

            uploadParams.File = new FileDescription(m_testPdfPath);
            var uploadResult2 = m_cloudinary.Upload(uploadParams, raw);

            var parameters = new ArchiveParams()
                                            .PublicIds(new List<string> { uploadResult1.PublicId, uploadResult2.PublicId })
                                            .ResourceType(raw)
                                            .Type(STORAGE_TYPE_PRIVATE)
                                            .UseOriginalFilename(true)
                                            .TargetTags(new List<string> { m_apiTag });
            var result = m_cloudinary.CreateArchive(parameters);
            Assert.AreEqual(2, result.FileCount);
        }

        [Test]
        public void TestCreateArchiveMultiplePublicIds()
        {
            // should support archiving based on multiple public IDs
            var parameters = UploadImageForArchiveAndPrepareParameters(GetMethodTag());
            var result = m_cloudinary.CreateArchive(parameters);

            Assert.AreEqual($"{parameters.TargetPublicId()}.{FILE_FORMAT_ZIP}", result.PublicId);
            Assert.AreEqual(1, result.FileCount);
        }

        [Test]
        public void TestCreateArchiveMultipleResourceTypes()
        {
            var tag = GetMethodTag();
            var upRes1 = UploadResourceForTestArchive<RawUploadParams>(tag);
            var upRes2 = UploadResourceForTestArchive<ImageUploadParams>(tag);
            var upRes3 = UploadResourceForTestArchive<VideoUploadParams>(tag);

            var fQPublicIds = new List<string>
            {
                upRes1.FullyQualifiedPublicId,
                upRes2.FullyQualifiedPublicId,
                upRes3.FullyQualifiedPublicId
            };

            var parameters = new ArchiveParams()
                .UseOriginalFilename(true)
                .TargetTags(new List<string> { tag, m_apiTag });

            var ex = Assert.Throws<ArgumentException>(() => m_cloudinary.CreateArchive(parameters));

            StringAssert.StartsWith("At least one of the following", ex.Message);

            parameters.ResourceType("auto").Tags(new List<string> { "tag" });

            ex = Assert.Throws<ArgumentException>(() => m_cloudinary.CreateArchive(parameters));

            StringAssert.StartsWith("To create an archive with multiple types of assets", ex.Message);

            parameters.ResourceType("").Tags(null).FullyQualifiedPublicIds(fQPublicIds);

            ex = Assert.Throws<ArgumentException>(() => m_cloudinary.CreateArchive(parameters));

            StringAssert.StartsWith("To create an archive with multiple types of assets", ex.Message);

            Assert.AreEqual(fQPublicIds, parameters.FullyQualifiedPublicIds());

            parameters.ResourceType("auto");

            var result = m_cloudinary.CreateArchive(parameters);

            Assert.AreEqual(3, result.FileCount);
        }

        [Test]
        public void TestCreateZip()
        {
            var parameters = UploadImageForArchiveAndPrepareParameters(GetMethodTag());
            var result = m_cloudinary.CreateZip(parameters);

            Assert.AreEqual($"{parameters.TargetPublicId()}.{FILE_FORMAT_ZIP}", result.PublicId);
            Assert.AreEqual(1, result.FileCount);
        }

        [Test]
        public void TestDownloadArchiveUrlForImage()
        {
            var archiveTag = GetMethodTag();
            var uploadResult = UploadResourceForTestArchive<ImageUploadParams>(archiveTag);
            var publicIds = new List<string> { uploadResult.PublicId };

            var archiveParams = new ArchiveParams().PublicIds(publicIds);
            var archiveUrl = m_cloudinary.DownloadArchiveUrl(archiveParams);

            Assert.True(UrlExists(archiveUrl));
        }

        [Test]
        public void TestDownloadArchiveUrlForVideo()
        {
            var archiveTag = GetMethodTag();
            var uploadResult = UploadResourceForTestArchive<VideoUploadParams>(archiveTag);
            var publicIds = new List<string> { uploadResult.PublicId };

            var resourceType = ApiShared.GetCloudinaryParam(ResourceType.Video);
            var archiveParams = new ArchiveParams().ResourceType(resourceType).PublicIds(publicIds);
            var archiveUrl = m_cloudinary.DownloadArchiveUrl(archiveParams);

            Assert.True(UrlExists(archiveUrl));
        }

        [Test]
        public void TestDownloadArchiveUrlForRaw()
        {
            var archiveTag = GetMethodTag();
            var uploadResult = UploadResourceForTestArchive<RawUploadParams>(archiveTag);
            var publicIds = new List<string> { uploadResult.PublicId };

            var resourceType = ApiShared.GetCloudinaryParam(ResourceType.Raw);
            var archiveParams = new ArchiveParams().ResourceType(resourceType).PublicIds(publicIds);
            var archiveUrl = m_cloudinary.DownloadArchiveUrl(archiveParams);

            Assert.True(UrlExists(archiveUrl));
        }

        [Test]
        public void TestDownloadZipForImage()
        {
            var archiveTag = GetMethodTag();
            UploadResourceForTestArchive<ImageUploadParams>(archiveTag);
            
            var archiveUrl = m_cloudinary.DownloadZip(archiveTag, null);

            Assert.True(UrlExists(archiveUrl));
        }

        [Test]
        public void TestDownloadZipForVideo()
        {
            var archiveTag = GetMethodTag();
            UploadResourceForTestArchive<VideoUploadParams>(archiveTag);
            var resourceType = ApiShared.GetCloudinaryParam(ResourceType.Video);

            var archiveUrl = m_cloudinary.DownloadZip(archiveTag, null, resourceType);

            Assert.True(UrlExists(archiveUrl));
        }

        private ArchiveParams UploadImageForArchiveAndPrepareParameters(string archiveTag)
        {
            UploadResourceForTestArchive<ImageUploadParams>($"{archiveTag},{m_apiTag}", true, 2.0);

            return new ArchiveParams().Tags(new List<string> { archiveTag, "non-existent-tag" }).
                TargetTags(new List<string> { m_apiTag }).TargetPublicId(GetUniquePublicId());
        }

        private RawUploadResult UploadResourceForTestArchive<T>(string archiveTag, bool useFileName = false, double imageWidth = 0.0)
            where T: RawUploadParams
        {
            var filesMap = new Dictionary<Type, string>
            {
                {typeof(ImageUploadParams), m_testImagePath},
                {typeof(VideoUploadParams), m_testVideoPath},
                {typeof(RawUploadParams), m_testPdfPath}
            };

            var uploadParams = Activator.CreateInstance<T>();
            uploadParams.File = new FileDescription(filesMap[typeof(T)]);
            uploadParams.Tags = $"{archiveTag},{m_apiTag}";
            uploadParams.UseFilename = useFileName;
            if (imageWidth > 0 && uploadParams is ImageUploadParams)
            {
                (uploadParams as ImageUploadParams).EagerTransforms = new List<Transformation>
                {
                    new Transformation().Crop("scale").Width(imageWidth)
                };
            }

            return uploadParams.GetType() != typeof(RawUploadParams) ? 
                m_cloudinary.Upload(uploadParams) :
                m_cloudinary.Upload(uploadParams, ApiShared.GetCloudinaryParam(ResourceType.Raw));
        }
    }
}
